### Title: Increase Azure deployment timeout and review Kudu warm-up failures

**Description:**
The deployment process for `worldleaders-api-prod` failed due to timeout errors and Kudu warm-up issues. Observations from the failed workflow ([view logs](https://github.com/victorsaly/WorldLeadersGame/actions/runs/16859477192/job/47757550944#step:9:1)):

1. Current timeout of 600 seconds appears insufficient. Suggest increasing to 900 seconds or more.
2. Kudu warm-up failed consistently, which may have contributed to delays.
3. The deployment command used (`az webapp deploy`) is deprecated and should be updated.

**Action Items**:
- Increase the deployment timeout.
- Investigate Kudu warm-up failures to improve deployment speed.
- Update the deployment command to the latest recommended version.