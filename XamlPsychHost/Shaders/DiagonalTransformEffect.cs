using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class DiagonalTransformEffect : ShaderEffect
    {
        private static readonly PixelShader Shader = new PixelShader { UriSource = new Uri("pack://application:,,,/Shaders/DiagonalTransform.ps") };

        public DiagonalTransformEffect()
        {
            PixelShader = Shader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(RGBAProperty);
        }

        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(nameof(Input), typeof(DiagonalTransformEffect), 0);
        public static readonly DependencyProperty RGBAProperty = DependencyProperty.Register(nameof(RGBA), typeof(Color), typeof(DiagonalTransformEffect), new UIPropertyMetadata(Colors.White, PixelShaderConstantCallback(0)));

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public Color RGBA
        {
            get { return (Color)GetValue(RGBAProperty); }
            set { SetValue(RGBAProperty, value); }
        }
    }
}
