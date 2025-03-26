using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ERPChatbot4.MLModels
{
    public class QuestionAnsweringModel
    {
        private readonly MLContext _mlContext;
        private PredictionEngine<InputData, OutputData> _predictionEngine;

        public QuestionAnsweringModel()
        {   
            _mlContext = new MLContext();

            var data = new List<InputData>
            {
                 new InputData { Question = "What is the invoice number?", Answer = @"Invoice\s*Number\s*:\s*(\w+)" },
                new InputData { Question = "What is the total amount?", Answer = @"Amount\s*:\s*\$?([\d,]+)" },
                new InputData { Question = "When is the due date?", Answer = @"Due\s*Date\s*:\s*([\w\s\d]+)" },
                new InputData { Question = "What is the payment status?", Answer = @"Status\s*:\s*(\w+)" },
              //  new InputData { Question = "What is the HR policy?", Answer = @"HR\s*Policy\s*:\s*(.*?)(?=(Invoice\s*Number|Purchase Order|Company Information|$))" },
              //Hr policies 
                new InputData { Question = "What is the HR policy?", Answer = @"HR\s*Policy\s*:\s*(.*?)(?=(Purchase Order|Company Information|$))" },
                new InputData { Question = "What is the leave policy?", Answer = @"Leave\s*Policy\s*:\s*(.*?)(?=(Attendance Policy|Remote Work Policy|$))" },
                new InputData { Question = "What is the attendance policy?", Answer = @"Attendance\s*Policy\s*:\s*(.*?)(?=(Remote Work Policy|$))" },
                new InputData { Question = "What is the remote work policy?", Answer = @"Remote\s*Work\s*Policy\s*:(.*?)(?=(Purchase Order|Company Information|$))"  }
            };

            //Loading the data and creating the pipeline
            var trainingData = _mlContext.Data.LoadFromEnumerable(data);  //Loads the list of sample data (data) into ML.NET as a dataset for training.

            // Using Multi-class Classification Trainer
            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("Answer")  //Converts the Answer column into a key for multi-class classification.
                          .Append(_mlContext.Transforms.Text.FeaturizeText("Features", "Question")) //Transforms the Question into a feature vector
                          .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Answer", "Features"))  //SDCA algo used to classify questions into categories.
                          .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            //Model training and prediction Engine creation
            try
            {
                var model = pipeline.Fit(trainingData);  //pipeline is trained on the provided data.



                _predictionEngine = _mlContext.Model.CreatePredictionEngine<InputData, OutputData>(model);  
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("NLP model initialization failed.", ex);
            }
        }

        public string PredictPattern(string question)
        {
            if (_predictionEngine == null)
            {
                throw new InvalidOperationException("NLP model is not initialized.");
            }

            var prediction = _predictionEngine.Predict(new InputData { Question = question });
            return prediction.Answer ?? string.Empty;
        }
    }
}
