using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
namespace intex2.Models;

public class PredictionService
{
    private readonly ILegoRepository _repo;
    
    public PredictionService(ILegoRepository repo)
    {
        _repo = repo;
    }

    public Order PredictFraud(Order order)
    {
        // Direct assignment for numeric values
        int? time = order.Time;
        float? amount = order.Amount;
        
        //Customer cust = _repo.
        
        //need to age somehow
        int transaction_shipping_match = 0;
        int residence_transaction_match = 0;
        
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
        
        return order;
    }
}