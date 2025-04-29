// NationalityToFlagConverter.cs
using System.Globalization;


namespace EsportsManager.Converters
{
    public class NationalityToFlagConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not string nationality)
                return ImageSource.FromFile("flag_placeholder.png");

            var imageName = nationality.ToLower() switch
            {
                "usa" => "us.png",
                "france" => "fr.png",
                "germany" => "de.png",
                "bulgaria" => "bg.png",
                "kazakhstan" => "kz.png",
                "serbia" => "rs.png",
                "sweden" => "se.png",
                "norway" => "no.png",
                "denmark" => "dk.png",
                "finland" => "fi.png",
                "poland" => "pl.png",
                "russia" => "ru.png",
                "south korea" => "kr.png",
                "canada" => "ca.png",
                "mexico" => "mx.png",
                "argentina" => "ar.png",
                "brazil" => "br.png",
                "greece" => "gr.png",
                "turkey" => "tr.png",
                "ukraine" => "ua.png",
                "romania" => "ro.png",
                "columbia" => "co.png",
                "india" => "in.png",
                "australia" => "au.png",
                "south africa" => "za.png",
                "china" => "cn.png",
                "japan" => "jp.png",
                "uk" => "gb.png",
                "spain" => "es.png",
                "netherlands" => "nl.png",
                "italy" => "it.png",
                "albania" => "al.png",
                "kosovo" => "xk.png",
                "mongolia" => "mn.png",
                "indonesia" => "id.png",
                "vietnam" => "vn.png",
                "philippines" => "ph.png",
                "am" => "a_m.png",
                "eu" => "e_u.png",
                "as" => "a_s.png",
                _ => "flag_placeholder.png"
            };

            return ImageSource.FromFile(imageName);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
