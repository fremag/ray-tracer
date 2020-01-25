using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ray_tracer.Cameras;
using ray_tracer_demos.Basic;
using ray_tracer_ui.Data;

namespace ray_tracer_ui.Pages
{
    public class RayTracingComponent : ComponentBase
    {
        [Inject]
        protected RayTracingService RayTracingService { get; set; }

        protected string SceneName
        {
            get => sceneName;
            set
            {
                sceneName = value;
                InitCameraParameters();
            }
        }

        protected readonly CameraParameters cameraParameters = new CameraParameters();
        protected readonly RenderParameters renderParameters = new RenderParameters();
        protected string img;
        protected string time;
        protected string progress;
        protected double progressValue;
        protected string speed;
        private string sceneName;

        protected void Start()
        {
            RayTracingService.Run(SceneName, cameraParameters, renderParameters);
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
                progressValue = 0;
                speed = "";
                return;
            }
            time = $"{stats.Time:hh\\:mm\\:ss}";
            progress = $"{stats.Progress:p2}";
            speed = $"{stats.Speed:n2} pix/s";
            progressValue = stats.Progress * 100;
        }
    
        protected override Task OnInitializedAsync()
        {
            SceneName = typeof(MengerSpongeScene).Name;
            RayTracingService.Clock += OnClock;
            return Task.CompletedTask;
        }

        private void OnClock(DateTime now)
        {
            Display();
            InvokeAsync(StateHasChanged);
        }

        public void InitCameraParameters()
        {
            var camParams = RayTracingService.CameraParameters(SceneName);
            var cameraParameter = camParams[0];
            cameraParameters.Height =  cameraParameter.Height;
            cameraParameters.Width =  cameraParameter.Width;
            cameraParameters.CameraX =  cameraParameter.CameraX;
            cameraParameters.CameraY =  cameraParameter.CameraY;
            cameraParameters.CameraZ =  cameraParameter.CameraZ;
            cameraParameters.LookX =  cameraParameter.LookX;
            cameraParameters.LookY =  cameraParameter.LookY;
            cameraParameters.LookZ =  cameraParameter.LookZ;
            InvokeAsync(StateHasChanged);
        }
    }
}