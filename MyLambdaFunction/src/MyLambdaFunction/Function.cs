using Amazon.CloudWatch.EMF.Logger;
using Amazon.CloudWatch.EMF.Model;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MyLambdaFunction;

public class Function
{
    public void FunctionHandler(string input, ILambdaContext context)
    {
        try
        {
            using (var logger = new MetricsLogger()) {
                // 名前空間を設定
                logger.SetNamespace("Canary");
                var dimensionSet = new DimensionSet();
                
                // ディメンション（ラベル）を設定
                dimensionSet.AddDimension("Service", "aggregator");
                logger.SetDimensions(dimensionSet);

                // メトリクスを記録
                logger.PutMetric("ProcessingLatency", 100, Unit.MILLISECONDS,StorageResolution.STANDARD);
                logger.PutMetric("Memory.HeapUsed", 1600424.0, Unit.BYTES, StorageResolution.STANDARD);
                logger.PutProperty("RequestId", "test-request-id");

                // メトリクスを送信
                logger.Flush();
            }

            context.Logger.LogLine($"Metrics sent successfully.");
        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"Error sending metrics: {ex.Message}");
            throw;
        }
    }
}
