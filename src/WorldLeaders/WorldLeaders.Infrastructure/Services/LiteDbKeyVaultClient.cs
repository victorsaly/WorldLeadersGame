using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LiteDB;
using System.Security.Cryptography;
using System.Text;
using WorldLeaders.Infrastructure.Configuration;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// LiteDB-based secure key vault for educational game encryption
/// Context: Educational game component for 12-year-old data security
/// Safety Requirements: Secure key storage, AES-256 encryption, child data protection
/// </summary>
public class LiteDbKeyVaultClient : IKeyVaultClient, IDisposable
{
    private readonly ILogger<LiteDbKeyVaultClient> _logger;
    private readonly LiteDatabase _database;
    private readonly ILiteCollection<EncryptionKeyDocument> _keys;
    private readonly ILiteCollection<EncryptedDataDocument> _encryptedData;
    private readonly string _masterKey;
    private bool _disposed = false;

    public LiteDbKeyVaultClient(
        IOptions<LiteDbOptions> liteDbOptions,
        IOptions<AzureKeyVaultOptions> keyVaultOptions,
        ILogger<LiteDbKeyVaultClient> logger)
    {
        _logger = logger;
        
        var connectionString = Path.Combine(
            Path.GetDirectoryName(liteDbOptions.Value.ConnectionString) ?? AppContext.BaseDirectory,
            "keyvault.db");
            
        // Ensure directory exists
        var directory = Path.GetDirectoryName(connectionString);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Use password protection for the key vault database
        var dbConnectionString = $"filename={connectionString};password={GetMasterPassword()}";
        _database = new LiteDatabase(dbConnectionString);
        
        _keys = _database.GetCollection<EncryptionKeyDocument>("encryption_keys");
        _encryptedData = _database.GetCollection<EncryptedDataDocument>("encrypted_data");

        // Create indexes for better performance
        _keys.EnsureIndex(x => x.KeyName, unique: true);
        _encryptedData.EnsureIndex(x => x.DataId, unique: true);
        _encryptedData.EnsureIndex(x => x.KeyName);

        _masterKey = keyVaultOptions.Value.MasterKey ?? "WorldLeadersEducationalGame2025!";

        _logger.LogInformation("LiteDB key vault initialized with secure database: {ConnectionString}", connectionString);
    }

    /// <summary>
    /// Encrypt data using stored or generated key
    /// </summary>
    public async Task<string> EncryptAsync(string data, string keyName)
    {
        try
        {
            _logger.LogInformation("Encrypting data with key: {KeyName}", keyName);

            // Get or create encryption key
            var keyDoc = await GetOrCreateKeyAsync(keyName);
            var key = DecryptKeyData(keyDoc.EncryptedKeyData);

            // Encrypt the data using AES-256
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(data);
            byte[] cipherBytes;

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();
                }
                cipherBytes = ms.ToArray();
            }

            // Combine IV and cipher text
            var result = new byte[aes.IV.Length + cipherBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);

            var encryptedBase64 = Convert.ToBase64String(result);

            // Store encrypted data reference
            var dataDoc = new EncryptedDataDocument
            {
                Id = ObjectId.NewObjectId(),
                DataId = Guid.NewGuid().ToString(),
                KeyName = keyName,
                EncryptedContent = encryptedBase64,
                CreatedAt = DateTime.UtcNow
            };

            _encryptedData.Insert(dataDoc);

            _logger.LogInformation("Data encrypted successfully with key: {KeyName}, DataId: {DataId}", 
                keyName, dataDoc.DataId);

            return dataDoc.DataId; // Return data ID for reference
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to encrypt data with key: {KeyName}", keyName);
            throw;
        }
    }

    /// <summary>
    /// Decrypt data using stored key
    /// </summary>
    public async Task<string> DecryptAsync(string dataId, string keyName)
    {
        try
        {
            _logger.LogInformation("Decrypting data with ID: {DataId}, key: {KeyName}", dataId, keyName);

            // Get encrypted data
            var dataDoc = _encryptedData.FindOne(x => x.DataId == dataId && x.KeyName == keyName);
            if (dataDoc == null)
            {
                throw new InvalidOperationException($"Encrypted data not found for ID: {dataId}");
            }

            // Get encryption key
            var keyDoc = _keys.FindOne(x => x.KeyName == keyName);
            if (keyDoc == null)
            {
                throw new InvalidOperationException($"Encryption key not found: {keyName}");
            }

            var key = DecryptKeyData(keyDoc.EncryptedKeyData);
            var encryptedBytes = Convert.FromBase64String(dataDoc.EncryptedContent);

            // Extract IV and cipher text
            using var aes = Aes.Create();
            aes.Key = key;

            var iv = new byte[aes.BlockSize / 8];
            Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
            aes.IV = iv;

            var cipherBytes = new byte[encryptedBytes.Length - iv.Length];
            Buffer.BlockCopy(encryptedBytes, iv.Length, cipherBytes, 0, cipherBytes.Length);

            // Decrypt the data
            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(cipherBytes);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cs);

            var plainText = await reader.ReadToEndAsync();

            _logger.LogInformation("Data decrypted successfully with key: {KeyName}", keyName);
            return plainText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decrypt data with ID: {DataId}, key: {KeyName}", dataId, keyName);
            throw;
        }
    }

    /// <summary>
    /// Create or rotate encryption key
    /// </summary>
    public async Task<string> CreateKeyAsync(string keyName, string keyType = "AES256")
    {
        try
        {
            _logger.LogInformation("Creating key: {KeyName} of type: {KeyType}", keyName, keyType);

            // Generate new 256-bit key
            byte[] keyBytes;
            switch (keyType.ToUpperInvariant())
            {
                case "AES256":
                case "AES":
                default:
                    keyBytes = new byte[32]; // 256 bits
                    RandomNumberGenerator.Fill(keyBytes);
                    break;
            }

            // Encrypt the key data with master key
            var encryptedKeyData = EncryptKeyData(keyBytes);

            var keyDoc = new EncryptionKeyDocument
            {
                Id = ObjectId.NewObjectId(),
                KeyName = keyName,
                KeyType = keyType,
                EncryptedKeyData = encryptedKeyData,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddYears(1),
                IsActive = true
            };

            // Remove any existing key with the same name
            _keys.DeleteMany(x => x.KeyName == keyName);
            
            // Insert new key
            _keys.Insert(keyDoc);

            _logger.LogInformation("Key created successfully: {KeyName}", keyName);
            
            // Use Task.Delay to make this properly async
            await Task.Delay(1);
            
            return keyDoc.Id.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create key: {KeyName}", keyName);
            throw;
        }
    }

    /// <summary>
    /// Validate key vault connectivity and permissions
    /// </summary>
    public async Task<bool> ValidateConnectionAsync()
    {
        try
        {
            // Test database connectivity by counting keys
            var keyCount = _keys.Count();
            
            _logger.LogInformation("Key vault connection validated successfully. Keys count: {KeyCount}", keyCount);
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Key vault connection validation failed");
            return false;
        }
    }

    /// <summary>
    /// Get key vault region for compliance verification
    /// </summary>
    public string GetRegion()
    {
        return "UK-LocalStorage"; // Local LiteDB storage
    }

    /// <summary>
    /// Get or create encryption key
    /// </summary>
    private async Task<EncryptionKeyDocument> GetOrCreateKeyAsync(string keyName)
    {
        var keyDoc = _keys.FindOne(x => x.KeyName == keyName && x.IsActive);
        
        if (keyDoc == null)
        {
            await CreateKeyAsync(keyName);
            keyDoc = _keys.FindOne(x => x.KeyName == keyName && x.IsActive);
        }

        if (keyDoc == null)
        {
            throw new InvalidOperationException($"Failed to create or retrieve key: {keyName}");
        }

        return keyDoc;
    }

    /// <summary>
    /// Encrypt key data using master key
    /// </summary>
    private string EncryptKeyData(byte[] keyData)
    {
        using var aes = Aes.Create();
        var masterKeyBytes = Encoding.UTF8.GetBytes(_masterKey.PadRight(32).Substring(0, 32));
        aes.Key = masterKeyBytes;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        byte[] cipherBytes;

        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(keyData, 0, keyData.Length);
                cs.FlushFinalBlock();
            }
            cipherBytes = ms.ToArray();
        }

        var result = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// Decrypt key data using master key
    /// </summary>
    private byte[] DecryptKeyData(string encryptedKeyData)
    {
        var encryptedBytes = Convert.FromBase64String(encryptedKeyData);
        
        using var aes = Aes.Create();
        var masterKeyBytes = Encoding.UTF8.GetBytes(_masterKey.PadRight(32).Substring(0, 32));
        aes.Key = masterKeyBytes;

        var iv = new byte[aes.BlockSize / 8];
        Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
        aes.IV = iv;

        var cipherBytes = new byte[encryptedBytes.Length - iv.Length];
        Buffer.BlockCopy(encryptedBytes, iv.Length, cipherBytes, 0, cipherBytes.Length);

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(cipherBytes);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        
        var keyData = new byte[32]; // 256-bit key
        cs.Read(keyData, 0, keyData.Length);
        
        return keyData;
    }

    /// <summary>
    /// Get master password for database encryption
    /// </summary>
    private static string GetMasterPassword()
    {
        // In production, this should come from secure configuration
        return Environment.GetEnvironmentVariable("WORLDLEADERS_DB_PASSWORD") ?? 
               "WorldLeadersEducationalGameSecurePassword2025!";
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _database?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// LiteDB document model for encryption keys
/// </summary>
public class EncryptionKeyDocument
{
    public ObjectId Id { get; set; } = ObjectId.NewObjectId();
    public string KeyName { get; set; } = string.Empty;
    public string KeyType { get; set; } = "AES256";
    public string EncryptedKeyData { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// LiteDB document model for encrypted data
/// </summary>
public class EncryptedDataDocument
{
    public ObjectId Id { get; set; } = ObjectId.NewObjectId();
    public string DataId { get; set; } = string.Empty;
    public string KeyName { get; set; } = string.Empty;
    public string EncryptedContent { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
