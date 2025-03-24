using Microsoft.ML.Data;

namespace ERPChatbot4.MLModels
{
    public class InputData
    {
        [LoadColumn(0)]
        public string Question { get; set; }

        [LoadColumn(1)]
        public string Answer { get; set; }
    }

    public class OutputData
    {
        [ColumnName("PredictedLabel")]
        public string Answer { get; set; }
    }
}
