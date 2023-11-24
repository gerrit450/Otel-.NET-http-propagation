using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Trace;

namespace Service_A.Controllers
{
    [Route("ServiceA")]
    public class ServiceAController : Controller
    {
        private readonly Tracer _tracer;

        public ServiceAController(Tracer tracer)
        {
            _tracer = tracer;
        }

        [HttpGet]
        [Route("Get")]
        public async Task Get()
        {
            var client = new HttpClient();

            using var span = _tracer.StartActiveSpan($"{nameof(ServiceAController)}.{nameof(Get)}");
            {
                span.SetAttribute("name", "service A");

                var context = span.Context;

                var traceState = context.TraceState;
                var traceId = context.TraceId;
                var spanId = context.SpanId;
                var traceFlags = context.TraceFlags;

                Request.Headers.Add("traceparent", $"{traceState}-{traceId}-{spanId}-{traceFlags}"); // doing it manually

                // Propagator.Inject(new PropagationContext(context, default), Request, InjectTraceContextIntoBasicProperties); // with propagator

                var response = await client.GetStringAsync($"https://localhost:7217/ServiceB/Get");
            }
        }
    }
}
