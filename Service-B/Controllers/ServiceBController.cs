using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Trace;

namespace Service_B.Controllers
{
    [Route("ServiceB")]
    public class ServiceBController : Controller
    {
        private readonly Tracer _tracer;
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

        public ServiceBController(Tracer tracer)
        {
            _tracer = tracer;
        }

        [HttpGet]
        [Route("Get")]
        public async Task Get()
        {
            var client = new HttpClient();
            var traceContext = Propagator.Extract(default, Request, HttpPropagation.HttpPropagation.ExtractTraceContextFromBasicProperties);
            var spanContext = new SpanContext(traceContext.ActivityContext);

            using var span = _tracer.StartActiveSpan($"{nameof(ServiceBController)}.{nameof(Get)}");
            {
                span.SetAttribute("name", "service B");

                var context = span.Context;

                var traceState = context.TraceState;
                var traceId = context.TraceId;
                var spanId = context.SpanId;
                var traceFlags = context.TraceFlags;

                Request.Headers.Remove("traceparent"); // removing traceparent header
                Request.Headers.Add("traceparent", $"{traceState}-{traceId}-{spanId}-{traceFlags}"); // doing it manually

                // Propagator.Inject(new PropagationContext(context, default), Request, InjectTraceContextIntoBasicProperties); // with propagator

                var response = await client.GetStringAsync($"http://localhost:61810/api/v1.5/Site/6"); 
            }
        }
    }
}
