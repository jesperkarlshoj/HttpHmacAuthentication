using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HttpHmacAuthentication.Tests
{
    public static class TestData
    {
        public static Root Root { get; } = new Root
        {
            Fixtures = new Fixtures
            {
                FixtureList = new List<Fixture>
                {
                    new Fixture
                    {
                        Input = new Input
                        {
                              Name ="GET 1",
                              Description ="Valid GET request",
                              Host ="example.acquiapipet.net",
                              Url ="https://example.acquiapipet.net/v1.0/task-status/133?limit=10",
                              Method ="GET",
                              ContentBody ="",
                              ContentType ="application/json",
                              ContentSha ="",
                              Timestamp =1432075982,
                              Realm ="Pipet service",
                              Id ="efdde334-fe7b-11e4-a322-1697f925ec7b",
                              Secret ="W5PeGMxSItNerkNFqQMfYiJvH14WzVJMy54CPoTAYoI=",
                              Nonce ="d1954337-5319-4821-8427-115542e08d10",
                              SignedHeaders = new List<string>(),
                              Headers =new Dictionary<string, string>()
                        },
                        Expectations = new Expectation
                        {
                              AuthorizationHeader = "acquia-http-hmac id=\"efdde334-fe7b-11e4-a322-1697f925ec7b\",nonce=\"d1954337-5319-4821-8427-115542e08d10\",realm=\"Pipet%20service\",signature=\"MRlPr/Z1WQY2sMthcaEqETRMw4gPYXlPcTpaLWS2gcc=\",version=\"2.0\"",
                              SignableMessage = "GET\nexample.acquiapipet.net\n/v1.0/task-status/133\nlimit=10\nid=efdde334-fe7b-11e4-a322-1697f925ec7b&nonce=d1954337-5319-4821-8427-115542e08d10&realm=Pipet%20service&version=2.0\n1432075982",
                              MessageSignature = "Z1WQY2sMthcaEqETRMw4gPYXlPcTpaLWS2gcc/e5yhapv1s=",
                              ResponseSignature  = "M4wYp1MKvDpQtVOnN7LVt9L8or4pKyVLhfUFVJxHemU=",
                              ResponseBody = "{\"id\": 133, \"status\": \"done\"}"
                        }
                    },
                    new Fixture
                    {
                        Input = new Input
                        {
                            Name = "GET 2",
                            Description = "Valid GET request",
                            Host = "example.acquiapipet.net",
                            Url = "https://example.acquiapipet.net/v1.0/task-status/145?limit=1",
                            Method = "GET",
                            ContentBody = "",
                            ContentType = "application/json",
                            ContentSha = "",
                            Timestamp = 1432075982,
                            Realm = "Pipet service",
                            Id = "615d6517-1cea-4aa3-b48e-96d83c16c4dd",
                            Secret = "TXkgU2VjcmV0IEtleSBUaGF0IGlzIFZlcnkgU2VjdXJl",
                            Nonce = "24c0c836-4f6c-4ed6-a6b0-e091d75ea19d",
                            SignedHeaders = new List<string>(),
                            Headers = new Dictionary<string, string>()
                        },
                        Expectations = new Expectation
                        {
                            AuthorizationHeader = "acquia-http-hmac id=\"615d6517-1cea-4aa3-b48e-96d83c16c4dd\",nonce=\"24c0c836-4f6c-4ed6-a6b0-e091d75ea19d\",realm=\"Pipet%20service\",signature=\"1Ku5UroiW1knVP6GH4l7Z4IuQSRxZO2gp/e5yhapv1s=\",version=\"2.0\"",
                            SignableMessage = "GET\nexample.acquiapipet.net\n/v1.0/task-status/145\nlimit=1\nid=615d6517-1cea-4aa3-b48e-96d83c16c4dd&nonce=24c0c836-4f6c-4ed6-a6b0-e091d75ea19d&realm=Pipet%20service&version=2.0\n1432075982",
                            MessageSignature = "1Ku5UroiW1knVP6GH4l7Z4IuQSRxZO2gp/e5yhapv1s=",
                            ResponseSignature = "C98MEJHnQSNiYCxmI4CxJegO62sGZdzEEiSXgSIoxlo=",
                            ResponseBody = "{\"id\": 145, \"status\": \"in-progress\"}"
                        }
                    },
                      new Fixture
                    {
                        Input = new Input
                        {
                              Name = "GET 3",
        Description = "Valid GET request with signed headers",
        Host = "example.pipeline.io",
        Url = "https://example.pipeline.io/api/v1/ci/pipelines",
        Method = "GET",
        ContentBody = "",
        ContentType = "application/json",
        ContentSha = "",
        Timestamp = 1432075982,
        Realm = "CIStore",
        Id = "e7fe97fa-a0c8-4a42-ab8e-2c26d52df059",
        Secret = "bXlzZWNyZXRzZWNyZXR0aGluZ3Rva2VlcA==",
        Nonce = "a9938d07-d9f0-480c-b007-f1e956bcd027",
        SignedHeaders = new List<string>() { "X-Custom-Signer1", "X-Custom-Signer2" },
        Headers = new Dictionary<string, string>{
            { "X-Custom-Signer1", "custom-1" },
          { "X-Custom-Signer2", "custom-2" }
        }
                        },
                        Expectations = new Expectation
                        {
          AuthorizationHeader = "acquia-http-hmac headers=\"X-Custom-Signer1%3BX-Custom-Signer2\",id=\"e7fe97fa-a0c8-4a42-ab8e-2c26d52df059\",nonce=\"a9938d07-d9f0-480c-b007-f1e956bcd027\",realm=\"CIStore\",signature=\"yoHiYvx79ssSDIu3+OldpbFs8RsjrMXgRoM89d5t+zA=\",version=\"2.0\"",
        SignableMessage = "GET\nexample.pipeline.io\n/api/v1/ci/pipelines\n\nid=e7fe97fa-a0c8-4a42-ab8e-2c26d52df059&nonce=a9938d07-d9f0-480c-b007-f1e956bcd027&realm=CIStore&version=2.0\nx-custom-signer1:custom-1\nx-custom-signer2:custom-2\n1432075982",
        MessageSignature = "yoHiYvx79ssSDIu3+OldpbFs8RsjrMXgRoM89d5t+zA=",
        ResponseSignature = "cUDFSS5tN5vBBS7orIfUag8jhkaGouBb/o8fstUvTF8=",
        ResponseBody = "[{\"pipeline_id\":\"39b5d58d-0a8f-437d-8dd6-4da50dcc87b7\",\"sitename\":\"enterprise-g1:sfwiptravis\",\"name\":\"pipeline.yml\",\"last_job_id\":\"810e4344-1bed-4fd0-a642-1ba17eb996d5\",\"last_branch\":\"validate-yaml\",\"last_requested\":\"2016-03-25T20:26:39.000Z\",\"last_finished\":null,\"last_status\":\"succeeded\",\"last_duration\":null}]"
                        }
                    },
                        new Fixture
                    {
                        Input = new Input
                        {
        Name = "POST 1",
        Description = "Valid POST request",
        Host = "example.acquiapipet.net",
        Url = "https://example.acquiapipet.net/v1.0/task",
        Method = "POST",
        ContentBody = "{\"method\":\"hi.bob\",\"params\":[\"5\",\"4\",\"8\"]}",
        ContentType = "application/json",
        ContentSha = "6paRNxUA7WawFxJpRp4cEixDjHq3jfIKX072k9slalo=",
        Timestamp = 1432075982,
        Realm = "Pipet service",
        Id = "efdde334-fe7b-11e4-a322-1697f925ec7b",
        Secret = "W5PeGMxSItNerkNFqQMfYiJvH14WzVJMy54CPoTAYoI=",
        Nonce = "d1954337-5319-4821-8427-115542e08d10",
        SignedHeaders = new List<string>(),
        Headers = new Dictionary<string, string>()
                        },
                        Expectations = new Expectation
                        {
        AuthorizationHeader = "acquia-http-hmac id=\"efdde334-fe7b-11e4-a322-1697f925ec7b\",nonce=\"d1954337-5319-4821-8427-115542e08d10\",realm=\"Pipet%20service\",signature=\"XDBaXgWFCY3aAgQvXyGXMbw9Vds2WPKJe2yP+1eXQgM=\",version=\"2.0\"",
        SignableMessage = "POST\nexample.acquiapipet.net\n/v1.0/task\n\nid=efdde334-fe7b-11e4-a322-1697f925ec7b&nonce=d1954337-5319-4821-8427-115542e08d10&realm=Pipet%20service&version=2.0\n1432075982\napplication/json\n6paRNxUA7WawFxJpRp4cEixDjHq3jfIKX072k9slalo=",
        MessageSignature = "XDBaXgWFCY3aAgQvXyGXMbw9Vds2WPKJe2yP+1eXQgM=",
        ResponseSignature = "LusIUHmqt9NOALrQ4N4MtXZEFE03MjcDjziK+vVqhvQ=",
        ResponseBody = ""
                        }
                    },
                          new Fixture
                    {
                        Input = new Input
                        {
 Name = "POST 2",
        Description = "Valid POST request with signed headers.",
        Host = "example.pipeline.io",
        Url = "https://example.pipeline.io/api/v1/ci/pipelines/39b5d58d-0a8f-437d-8dd6-4da50dcc87b7/start",
        Method = "POST",
        ContentBody = "{\"cloud_endpoint\":\"https://cloudapi.acquia.com/v1\",\"cloud_user\":\"example@acquia.com\",\"cloud_pass\":\"password\",\"branch\":\"validate\"}",
        ContentType = "application/json",
        ContentSha = "2YGTI4rcSnOEfd7hRwJzQ2OuJYqAf7jzyIdcBXCGreQ=",
        Timestamp = 1449578521,
        Realm = "CIStore",
        Id = "e7fe97fa-a0c8-4a42-ab8e-2c26d52df059",
        Secret = "bXlzZWNyZXRzZWNyZXR0aGluZ3Rva2VlcA==",
        Nonce = "a9938d07-d9f0-480c-b007-f1e956bcd027",
        SignedHeaders = new List<string> { "X-Custom-Signer1", "X-Custom-Signer2" },
                Headers = new Dictionary<string, string>{
            { "X-Custom-Signer1", "custom-1" },
          { "X-Custom-Signer2", "custom-2" }
        }
                        },
                        Expectations = new Expectation
                        {
    AuthorizationHeader = "acquia-http-hmac headers=\"X-Custom-Signer1%3BX-Custom-Signer2\",id=\"e7fe97fa-a0c8-4a42-ab8e-2c26d52df059\",nonce=\"a9938d07-d9f0-480c-b007-f1e956bcd027\",realm=\"CIStore\",signature=\"0duvqeMauat7pTULg3EgcSmBjrorrcRkGKxRDtZEa1c=\",version=\"2.0\"",
        SignableMessage = "POST\nexample.pipeline.io\n/api/v1/ci/pipelines/39b5d58d-0a8f-437d-8dd6-4da50dcc87b7/start\n\nid=e7fe97fa-a0c8-4a42-ab8e-2c26d52df059&nonce=a9938d07-d9f0-480c-b007-f1e956bcd027&realm=CIStore&version=2.0\nx-custom-signer1:custom-1\nx-custom-signer2:custom-2\n1449578521\napplication/json\n2YGTI4rcSnOEfd7hRwJzQ2OuJYqAf7jzyIdcBXCGreQ=",
        MessageSignature = "0duvqeMauat7pTULg3EgcSmBjrorrcRkGKxRDtZEa1c=",
        ResponseSignature = "SlOYi3pUZADkzU9wEv7kw3hmxjlEyMqBONFEVd7iDbM=",
        ResponseBody = "\"57674bb1-f2ce-4d0f-bfdc-736a78aa027a\""
                        }
                    }
                }
            }
        };
    }

    public class Input
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("host")]
        public string Host { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("ContentBody")]
        public string ContentBody { get; set; }

        [JsonPropertyName("ContentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("ContentSha")]
        public string ContentSha { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("realm")]
        public string Realm { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("secret")]
        public string Secret { get; set; }

        [JsonPropertyName("nonce")]
        public string Nonce { get; set; }

        [JsonPropertyName("SignedHeaders")]
        public List<string> SignedHeaders { get; set; }

        [JsonPropertyName("headers")]
        public Dictionary<string, string> Headers { get; set; }
    }

    public class Expectation
    {
        [JsonPropertyName("AuthorizationHeader")]
        public string AuthorizationHeader { get; set; }

        [JsonPropertyName("SignableMessage")]
        public string SignableMessage { get; set; }

        [JsonPropertyName("MessageSignature")]
        public string MessageSignature { get; set; }

        [JsonPropertyName("ResponseSignature")]
        public string ResponseSignature { get; set; }

        [JsonPropertyName("ResponseBody")]
        public string ResponseBody { get; set; }
    }

    public class Fixture
    {
        [JsonPropertyName("input")]
        public Input Input { get; set; }

        [JsonPropertyName("expectations")]
        public Expectation Expectations { get; set; }
    }

    public class Fixtures
    {
        [JsonPropertyName("2.0")]
        public List<Fixture> FixtureList { get; set; }
    }

    public class Root
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("spec_versions")]
        public List<string> SpecVersions { get; set; }

        [JsonPropertyName("fixtures")]
        public Fixtures Fixtures { get; set; }
    }


}
