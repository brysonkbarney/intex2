using intex2.Controllers;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
namespace intex2.Models;

public class PredictionService
{
    private readonly ILegoRepository _repo;
    private readonly InferenceSession _session;
    public PredictionService(ILegoRepository repo)
    {
        _repo = repo;
        
        // Get the absolute path to the directory containing the executable
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        // Construct the path to the model file relative to the base directory
        var modelPath = Path.Combine(baseDir, "decision_tree_model.onnx");
        _session = new InferenceSession(
            modelPath);
    }
    

    public int PredictFraud(Order order)
    {
        // Direct assignment for numeric values
        int time = order.Time ?? 0;
        float amount = order.Amount ?? 0;

        Customer cust = _repo.GetCustomerByID(order.CustomerId);
        
        //age
        int age = (int)Math.Round(cust.Age ?? 0);
        
        int transaction_shipping_match = order.CountryOfTransaction == order.ShippingAddress ? 1 : 0;
        int residence_transaction_match = order.CountryOfTransaction == cust.CountryOfResidence ? 1 : 0;
        
        // One-hot encoding for categorical variables
        // Day of week
        int day_of_week_Fri = order.DayOfWeek == "Fri" ? 1 : 0;
        int day_of_week_Mon = order.DayOfWeek == "Mon" ? 1 : 0;
        int day_of_week_Sat = order.DayOfWeek == "Sat" ? 1 : 0;
        int day_of_week_Sun = order.DayOfWeek == "Sun" ? 1 : 0;
        int day_of_week_Thu = order.DayOfWeek == "Thu" ? 1 : 0;
        int day_of_week_Tue = order.DayOfWeek == "Tue" ? 1 : 0;
        int day_of_week_Wed = order.DayOfWeek == "Wed" ? 1 : 0;
        

        // Entry mode
        int entry_mode_CVC = order.EntryMode == "CVC" ? 1 : 0;
        int entry_mode_PIN = order.EntryMode == "PIN" ? 1 : 0;

        // Type of transaction
        int type_of_transaction_POS = 0;

        // Country of transaction
        int country_of_transaction_China = order.CountryOfTransaction == "China" ? 1 : 0;
        int country_of_transaction_India = order.CountryOfTransaction == "India" ? 1 : 0;
        int country_of_transaction_Russia = order.CountryOfTransaction == "Russia" ? 1 : 0;
        int country_of_transaction_USA = order.CountryOfTransaction == "USA" ? 1 : 0;
        
        //shipping address
        int shipping_address_China = order.ShippingAddress == "China" ? 1 : 0;
        int shipping_address_India = order.ShippingAddress == "India" ? 1 : 0;
        int shipping_address_Russia = order.ShippingAddress == "Russia" ? 1 : 0;
        int shipping_address_USA = order.ShippingAddress == "USA" ? 1 : 0;
        
        //bank
        int bank_Barclays = order.Bank == "Barclays" ? 1 : 0;
        int bank_HSBC = order.Bank == "HSBC" ? 1 : 0;
        int bank_Halifax = order.Bank == "Halifax" ? 1 : 0;
        int bank_Lloyds = order.Bank == "Lloyds" ? 1 : 0;
        int bank_Metro = order.Bank == "Metro" ? 1 : 0;
        int bank_Monzo = order.Bank == "Monzo" ? 1 : 0;
        int bank_RBS = order.Bank == "RBS" ? 1 : 0;
        
        //type of card
        int type_of_card_MasterCard = order.TypeOfCard == "MasterCard" ? 1 : 0;
        int type_of_card_Visa = order.TypeOfCard == "Visa" ? 1 : 0;
        
        //gender
        int gender_F = cust.Gender == "F" ? 1 : 0;
        
        // Dictionary mapping the numeric prediction to an animal type
        var class_type_dict = new Dictionary<int, string>
        {
            { 0, "non-fraudulent" },
            { 1, "fraudulent" }
        };
        var input = new List<float> { time, amount, age, transaction_shipping_match, residence_transaction_match, day_of_week_Fri, day_of_week_Mon, day_of_week_Sat, day_of_week_Sun, day_of_week_Thu, day_of_week_Tue, day_of_week_Wed, entry_mode_CVC, entry_mode_PIN, type_of_transaction_POS, country_of_transaction_China, country_of_transaction_India, country_of_transaction_Russia, country_of_transaction_USA, shipping_address_China, shipping_address_India, shipping_address_Russia, shipping_address_USA, bank_Barclays, bank_HSBC, bank_Halifax, bank_Lloyds, bank_Metro, bank_Monzo, bank_RBS, type_of_card_MasterCard, type_of_card_Visa, gender_F
        };
        var inputTensor = new DenseTensor<float>(input.ToArray(), new[] { 1, input.Count });

        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("float_input", inputTensor)
        };

        using (var results = _session.Run(inputs)) // makes the prediction with the inputs from the form
        {
            var prediction = results.FirstOrDefault(item => item.Name == "output_label")?.AsTensor<long>().ToArray();
            if (prediction != null && prediction.Length > 0)
            {
                // Use the prediction to get the animal type from the dictionary
                var fraudPrediction = class_type_dict.GetValueOrDefault((int)prediction[0], "Unknown");
            }
            return (int)prediction[0];
        }
    }
}