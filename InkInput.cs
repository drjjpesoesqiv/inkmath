using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input.Inking;
using Windows.UI.Input.Inking.Analysis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InkMath
{
    class InkInput
    {
        InkCanvas inkCanvas;
        InkAnalyzer inkAnalyzer;
        DispatcherTimer recoTimer;
        Action<string> resultCallback;

        public InkInput(InkCanvas inkCanvas, Action<string> resultCallback)
        {
            this.inkCanvas = inkCanvas;
            this.resultCallback = resultCallback;

            inkAnalyzer = new InkAnalyzer();

            this.inkCanvas.InkPresenter.InputDeviceTypes =
                Windows.UI.Core.CoreInputDeviceTypes.Pen |
                Windows.UI.Core.CoreInputDeviceTypes.Touch;

            InkDrawingAttributes drawingAttributes = new InkDrawingAttributes();
            drawingAttributes.Color = Windows.UI.Colors.Black;
            drawingAttributes.Size = new Windows.Foundation.Size(4, 4);
            drawingAttributes.IgnorePressure = true;
            this.inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);

            this.inkCanvas.InkPresenter.StrokesCollected +=
                SolutionInkCanvas_StrokesCollected;

            this.inkCanvas.InkPresenter.StrokeInput.StrokeStarted +=
                SolutionInkCanvas_StrokeStarted;

            recoTimer = new DispatcherTimer();
            recoTimer.Interval = TimeSpan.FromSeconds(1);
            recoTimer.Tick += recoTimer_TickAsync;
        }

        public void SolutionInkCanvas_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            recoTimer.Stop();

            foreach (var stroke in args.Strokes)
            {
                inkAnalyzer.AddDataForStroke(stroke);
                inkAnalyzer.SetStrokeDataKind(stroke.Id, InkAnalysisStrokeKind.Writing);
            }

            recoTimer.Start();
        }

        public void SolutionInkCanvas_StrokeStarted(InkStrokeInput sender, Windows.UI.Core.PointerEventArgs args)
        {
            recoTimer.Stop();
        }

        public async void recoTimer_TickAsync(object sender, object e)
        {
            recoTimer.Stop();
            if (!inkAnalyzer.IsAnalyzing)
            {
                InkAnalysisResult result = await inkAnalyzer.AnalyzeAsync();

                if (result.Status == InkAnalysisStatus.Updated)
                {
                    var inkwordNodes =
                        inkAnalyzer.AnalysisRoot.FindNodes(
                            InkAnalysisNodeKind.InkWord);

                    foreach (InkAnalysisInkWord node in inkwordNodes)
                    {
                        string recoText = node.RecognizedText;

                        //
                        this.resultCallback(recoText);

                        foreach (var strokeId in node.GetStrokeIds())
                        {
                            var stroke =
                                this.inkCanvas.InkPresenter.StrokeContainer.GetStrokeById(strokeId);
                            stroke.Selected = true;
                        }

                        inkAnalyzer.RemoveDataForStrokes(node.GetStrokeIds());
                    }

                    this.inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                }
            }
            else
            {
                recoTimer.Start();
            }
        }
    }
}
