using System;

namespace ServerApp.Helpers
{
    public static class ExtensionMethods
    {
        public static int CalculateAge(this DateTime dateOfBirth) // this DateTime dateOfBirth => DateOfBirth.Calc..ge()
        {
            int age = 0;

            age = DateTime.Now.Year - dateOfBirth.Year;

            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)   // yıl içerisindeki gün bilgisi, doğum gününkinden küçükse o yıl çıkarılır..
                age-=1; 

            return age;
        }
    }
}