using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ray_tracer;
using ray_tracer_demos;
using ray_tracer_ui.Data;

namespace ray_tracer_ui.Pages
{
    public class RayTracingComponent : ComponentBase
    {
        [Inject]
        protected RayTracingService RayTracingService { get; set; }

        protected string sceneName;
        protected readonly CameraParameters cameraParameters = new CameraParameters();
        protected readonly RenderParameters renderParameters = new RenderParameters();
        protected string img;
        protected string time;
        protected string progress;
        protected string speed;
    
        protected void Start()
        {
            RayTracingService.Run(sceneName, cameraParameters, renderParameters);
        }
    
        protected void Stop()
        {
            RayTracingService.Stop();
        }
    
        protected async void Display()
        {
            img = await RayTracingService.GetImage();
            var stats = RayTracingService.GetStatistics();
            if (stats == null)
            {
                time = "";
                progress = "";
                speed = "";
                return;
            }
            time = $"{stats.Time:hh\\:mm\\:ss}";
            progress = $"{stats.Progress:p2}";
            speed = $"{stats.Speed:n2} pix/s";
        }
    
        protected override Task OnInitializedAsync()
        {
            sceneName = typeof(MengerSpongeScene).Name;
            RayTracingService.Clock += OnClock;
            return Task.CompletedTask;
        }

        private void OnClock(DateTime now)
        {
            Display();
            InvokeAsync(StateHasChanged);
        }
    }
}