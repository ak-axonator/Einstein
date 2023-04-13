using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LeadMagicLambda;

[JSONSerializable(typeof(APIGatewayHttpAPIV2ProxyRequest))]
[JSONSerializable(typeof(APIGatewayHttpAPIV2ProxyResponse))]
public class LeadData : JSONSerializerContext
{
    // [DynamoDBHashKey]
    public string id { get; set; }

    // [DynamoDBProperty]
    public string lead_email { get; set; }

    // [DynamoDBProperty]
    public string name { get; set; }

    // [DynamoDBProperty]
    public string email { get; set; }

    // [DynamoDBProperty]
    public string industry { get; set; }

    // [DynamoDBProperty]
    public string requirements { get; set; }
}
public class Function
{
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public string FunctionHandler(LeadData input, ILambdaContext context)
    {
        return input.name;
    }
}
