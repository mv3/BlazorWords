using BlazorWords.Models;
using BlazorWords.Constants;

namespace BlazorWords.Data
{
    public class Dictionary
    {
        public Dictionary()
        {
            LoadGuessWords();
        }

        public List<GuessWord> GuessWords { get; set; } = new();


        private void LoadGuessWords()
        {


            GuessWords.Add(new(0, "OWES", "A liability is something a person or company OWES, usually a sum of money.", "A liability is what a company...", "https://www.investopedia.com/terms/l/liability.asp"));
            GuessWords.Add(new(1, "CASH", "CASH is legal tender—currency or coins—that can be used to exchange goods, debt, or services. Sometimes it also includes the value of assets that can be easily converted into cash immediately.", "Legal tender.", "https://www.investopedia.com/terms/c/cash.asp"));
            GuessWords.Add(new(2, "ASSET", "An ASSET is a resource with economic value that an individual, corporation, or country owns or controls with the expectation that it will provide a future benefit. Assets are reported on a company's balance sheet.", "What a company owns.", "https://www.investopedia.com/terms/a/asset.asp"));
            GuessWords.Add(new(3, "MONEY", "Interest is the monetary charge for the privilege of borrowing money, typically expressed as an annual percentage rate (APR). Interest is the amount of MONEY a lender or financial institution receives for lending out money.", "Interest is the charge for borrowing...", "https://www.investopedia.com/terms/i/interest.asp"));
            GuessWords.Add(new(4, "EQUITY", "EQUITY, typically referred to as shareholders' equity (or owners' equity for privately held companies), represents the amount of money that would be returned to a company's shareholders if all of the assets were liquidated and all of the company's debt was paid off in the case of liquidation.", "What a company is worth.", "https://www.investopedia.com/terms/e/equity.asp"));
            GuessWords.Add(new(5, "PROFIT", "PROFIT describes the financial benefit realized when revenue generated from a business activity exceeds the expenses, costs, and taxes involved in sustaining the activity in question.", "Total Revenue - Total Expenses.", "https://www.investopedia.com/terms/p/profit.asp"));
            GuessWords.Add(new(6, "EBITDA", "EBITDA, or earnings before interest, taxes, depreciation, and amortization, is a measure of a company’s overall financial performance and is used as an alternative to net income in some circumstances.", "Cash Flow", "https://www.investopedia.com/terms/e/ebitda.asp"));
            GuessWords.Add(new(7, "EXPENSE", "An EXPENSE is the cost of operations that a company incurs to generate revenue. As the popular saying goes, “it costs money to make money.”", "Cost", "https://www.investopedia.com/terms/e/expense.asp"));
            GuessWords.Add(new(8, "REVENUE", "REVENUE is the money generated from normal business operations, calculated as the average sales price times the number of units sold. It is the top line (or gross income) figure from which costs are subtracted to determine net income.", "Also known as Sales or Top Line", "https://www.investopedia.com/terms/r/revenue.asp"));
        }
    }
}
