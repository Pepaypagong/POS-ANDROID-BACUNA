using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Android.Text;

namespace POS_ANDROID_BACUNA
{
    public class CurrencyTextWatcher : Java.Lang.Object, ITextWatcher
    {
        private EditText mEditText;
        private Context mContext;
        private string mCurrencySign;

        public CurrencyTextWatcher(EditText _mEditText, Context _context, string _currencySign)
        {
            mEditText = _mEditText;
            mContext = _context;
            mCurrencySign = _currencySign;
        }

        public void AfterTextChanged(IEditable s)
        {

        }
        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {

        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            string rawString;
            string firstString = "";
            try
            {
                firstString = s.ToString().Substring(0, 1);
            }
            catch (System.Exception)
            {
                firstString = "";
            }
                
            if (firstString != mCurrencySign)
            {
                try
                {
                    rawString = s.ToString().Substring(1, s.ToString().Length - 1);
                }
                catch (System.Exception)
                {
                    rawString = s.ToString();
                }
            }
            else
            {
                rawString = s.ToString();
            }
            if (s.ToString() == mCurrencySign + " 0.0")
            {
                mEditText.Text = mCurrencySign + " 0.00";
                mEditText.SetSelection(mEditText.Text.Length);
            }
            string value = rawString.Replace(" ","").Replace(",", "").Replace(mCurrencySign, "").Replace(".", "").TrimStart('0');
            string value2 = value == "" ? "0" : value;
            decimal ul;
            if (decimal.TryParse(value2, out ul))
            {
                ul /= 100;
                mEditText.RemoveTextChangedListener(this);
                string formatted = mCurrencySign + " " + string.Format("{0:n}", ul);
                mEditText.Text = formatted;
                mEditText.SetSelection(formatted.Length);
                mEditText.AddTextChangedListener(this);
            }
        }

    }
}