using Common;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BatchController
{
    public class BatchControllerTask
    {
        public BatchControllerTask()
        {
        }

        public void Run()
        {
            EstablishAsync().GetAwaiter().GetResult();
        }
                
        public async Task EstablishAsync()
        {
            AuthenticationContext authContext = new AuthenticationContext(Common.Constants.AuthorityUri);
            AuthenticationResult authResult = await authContext.AcquireTokenAsync(Common.Constants.BatchResourceUri, 
                new ClientCredential(Common.Constants.ClientId, Common.Constants.ClientKey));

            var cred = new BatchTokenCredentials(Common.Constants.BatchAccountUrl, authResult.AccessToken);

            using (BatchClient batchClient = BatchClient.Open(cred))
            {
                var jobId = $"Processor_{DateTime.UtcNow.Ticks}";
                var pool = batchClient.PoolOperations.GetPool(Common.Constants.PoolName);
                Console.WriteLine($"Creating job [{jobId}] on pool [{Common.Constants.PoolName}]");
                CloudJob job = batchClient.JobOperations.CreateJob();
                job.Id = jobId;
                job.PoolInformation = new PoolInformation { PoolId = Common.Constants.PoolName };
                job.Commit();

                var tasks = new List<CloudTask>();
                for (int i = 0; i < pool.TargetDedicatedComputeNodes.Value; i++)
                {
                    var taskId = $"{jobId}_{i}";
                    var taskCommandLine = "dotnet /mnt/batch/tasks/startup/wd/Consumer.dll";
                    var task = new CloudTask(taskId, taskCommandLine);
                    tasks.Add(task);
                }
                batchClient.JobOperations.AddTask(jobId, tasks);

                job.Refresh();
                job.OnAllTasksComplete = Microsoft.Azure.Batch.Common.OnAllTasksComplete.TerminateJob;
                job.CommitChanges();
            }
        }
    }
}