namespace MEungblut.TestUtility.DummyServices
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Nancy;

    using Newtonsoft.Json;

    namespace ME
    {
        public class DummyHttpService : NancyModule
        {
            private static readonly Dictionary<string, Response> ResponsesToSendBack = new Dictionary<string, Response>();
            private static Dictionary<string, string> Requests = new Dictionary<string, string>();
            private static Dictionary<string, dynamic> RequestParameters = new Dictionary<string, dynamic>();
            private static readonly Dictionary<string, HttpVerb> routes = new Dictionary<string, HttpVerb>();
            private static readonly Dictionary<string, string> websocketAddresses = new Dictionary<string, string>();


            private static string webSocketServer;

            private static string webSocketPort;

            public DummyHttpService()
            {
                foreach (var route in routes)
                {
                    if (route.Value == HttpVerb.Put)
                    {
                        this.Put[route.Key, true] = async (parameters, ct) =>
                        {
                            return Log(route.Key, parameters);
                        };
                    }

                    if (route.Value == HttpVerb.Post)
                    {
                        this.Post[route.Key, true] = async (parameters, ct) =>
                        {
                            return Log(route.Key, parameters);
                        };
                    }

                    this.Get[route.Key, true] = async (parameters, ct) =>
                    {
                        return Log(route.Key, parameters);
                    };
                }

                foreach (var websocketAddress in websocketAddresses)
                {
                    this.Get[websocketAddress.Key] = parameters =>
                    {
                        return websocketAddress.Value;
                    };
                }

                this.Get["/dummywebsocket/configuration"] = parameters =>
                {
                    return string.Format("{0}:{1}", webSocketServer, webSocketPort);
                };
            }

            private dynamic Log(string resource, dynamic parameters)
            {
                Console.WriteLine("Received request for resource " + resource);
                var body = this.Request.Body.ReadAsString();
                Requests[resource] = body;
                Console.WriteLine("Body was " + body);
                RequestParameters[resource] = parameters;
                return ResponsesToSendBack[resource];
            }

            public static void ClearReceivedData()
            {
                Requests = new Dictionary<string, string>();
                RequestParameters = new Dictionary<string, dynamic>();
            }

            public static void RegisterRoute(string route, HttpVerb verb)
            {
                if (routes.ContainsKey(route))
                    return;

                Console.WriteLine("Added route " + route + " for verb " + verb.ToString());

                routes.Add(route, verb);
            }

            public static void AddWebsocketLocation(string route, string location)
            {
                websocketAddresses.Add(route, location);
            }

            public static void AddResponseToReturnFromResource(string resource, Response response)
            {
                ResponsesToSendBack[resource] = response;
            }

            public static string GetLastRequestSentToResource(string resource)
            {
                return Requests[resource];
            }

            public static string GetUrlParameter(string resource, string parameter)
            {
                return RequestParameters[resource][parameter];
            }

            public static T WaitForServiceToBeSentARequestFor<T>(TimeSpan timeAfterWhichToFinishWaiting, string receivingResource)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();

                while (true)
                {
                    if (watch.Elapsed > timeAfterWhichToFinishWaiting) break;

                    string request;
                    if (!Requests.TryGetValue(receivingResource, out request)) continue;

                    T deserialisedType;
                    try
                    {
                        deserialisedType = JsonConvert.DeserializeObject<T>(request);
                    }
                    catch { continue; }

                    if (deserialisedType != null)
                        return deserialisedType;

                    Task.Delay(TimeSpan.FromMilliseconds(250));
                }

                throw new TimeoutException("Didn't receive the requested type in time " + typeof(T).ToString());
            }

            public static void SetWebServerLocation(string server, string port)
            {
                webSocketPort = port;
                webSocketServer = server;
            }
        }
    }

}
