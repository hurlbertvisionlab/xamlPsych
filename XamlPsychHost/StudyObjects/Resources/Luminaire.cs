using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Ledmotive;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class Luminaire : DependencyObject, ILogInfo
    {
        public LightHub Hub { get; set; }

        public Int32Collection IDs { get; set; }

        public async Task Show(LuminaireColor color, double scale = 1.0)
        {
            if (Hub == null)
                throw new InvalidOperationException("No Light Hub assigned to this luminaire.");

            // TODO: create multicast group
            // TODO: support 0.0-1.0 amplitudes

            ushort[] amplitudes = new ushort[color.Amplitudes.Count];

            for (int i = 0; i < amplitudes.Length; i++)
                amplitudes[i] = (ushort)Math.Round(color.Amplitudes[i] * scale, 0);

            foreach (int id in IDs)
                await Hub.SetSpectrumA((ushort)id, amplitudes);
        }

        public async Task Dim(double level)
        {
            if (Hub == null)
                throw new InvalidOperationException("No Light Hub assigned to this luminaire.");

            foreach (int id in IDs)
                await Hub.SetDimmingLevel((ushort)id, (int)level);
        }

        public string ToLogString(StudyContext context)
        {
            return IDs.ToString();
        }
    }
}
