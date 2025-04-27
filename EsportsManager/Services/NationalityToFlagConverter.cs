// NationalityToFlagConverter.cs
using System.Globalization;

public class NationalityToFlagConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string nationality)
            return null;

        var imageName = nationality.ToLower() switch
        {
            "USA" => "us.png",
            "France" => "fr.png",
            "Germany" => "de.png",
            "Bulgaria" => "bg.png",
            "Kazakhstan" => "kz.png",
            "Serbia" => "sr.png",
            "Sweden" => "se.png",
            "Norway" => "no.png",
            "Denmark" => "dk.png",
            "Finland" => "fi.png",
            "Poland" => "pl.png",
            "Russia" => "ru.png",
            "South Korea" => "kr.png",
            "Canada" => "ca.png",
            "Mexico" => "mx.png",
            "Argentina" => "ar.png",
            "Brazil" => "br.png",
            "Greece" => "gr.png",
            "Turkey" => "tr.png",
            "Ukraine" => "ua.png",
            "Romania" => "ro.png",
            "Columbia" => "co.png",
            "India" => "in.png",
            "Australia" => "au.png",
            "South Africa" => "za.png",
            "China" => "cn.png",
            "Japan" => "jp.png",
            "UK" => "uk.png",
            "Spain" => "es.png",
            "Netherlands" => "nl.png",
            "Italy" => "it.png",
            "Albania" => "al.png",
            "Kosovo" => "xk.png",
            //"" => ".png",
            _ => "flag_placeholder.png"
        };

        return ImageSource.FromFile(imageName);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}