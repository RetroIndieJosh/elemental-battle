using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

static public class AlakajamExtensions
{
    // Graphic

    public static IEnumerator FadeInCoroutine( this Graphic a_image, float a_fadeInTime ) {
        return FadeCoroutineHandler( a_image, a_fadeInTime, true );
    }

    public static IEnumerator FadeOutCoroutine( this Graphic a_image, float a_fadeOutTime ) {
        return FadeCoroutineHandler( a_image, a_fadeOutTime, false );
    }

    public static IEnumerator FadeInOutCoroutine( this Graphic a_image, float a_fadeInTime, float a_fadeOutTime = 0f ) {
        if ( a_fadeOutTime <= 0f ) a_fadeOutTime = a_fadeInTime;

        yield return FadeCoroutineHandler( a_image, a_fadeOutTime, true );
        //yield return new WaitForSeconds( a_fadeOutTime );

        yield return FadeCoroutineHandler( a_image, a_fadeInTime, false );
        //yield return new WaitForSeconds( a_fadeInTime );
    }

    public static IEnumerator FadeOutInCoroutine( this Graphic a_image, float a_fadeOutTime, float a_fadeInTime = 0f ) {
        if ( a_fadeInTime <= 0f ) a_fadeInTime = a_fadeOutTime;

        FadeCoroutineHandler( a_image, a_fadeOutTime, true );
        yield return new WaitForSeconds( a_fadeOutTime );

        FadeCoroutineHandler( a_image, a_fadeInTime, false );
        yield return new WaitForSeconds( a_fadeInTime );
    }

    public static IEnumerator FadeCoroutineHandler( this Graphic a_graphic, float a_fadeTime, bool a_fadeIn ) {
        var timeElapsed = 0f;
        while ( timeElapsed < a_fadeTime ) {
            timeElapsed += Time.deltaTime;
            var t = timeElapsed / a_fadeTime;
            if ( a_fadeIn ) a_graphic.color = Color.Lerp( Color.clear, Color.black, t );
            else a_graphic.color = Color.Lerp( Color.black, Color.clear, t );
            yield return null;
        }
    }

    // Color

    public static string ToHexString( this Color c ) {
        var r = Mathf.FloorToInt( c.r * 255 );
        var g = Mathf.FloorToInt( c.g * 255 );
        var b = Mathf.FloorToInt( c.b * 255 );
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    // List<T>

    public static T GetRandomElement<T>( this List<T> a_list ) {
        var roll = Random.Range( 0, a_list.Count );
        return a_list[roll];
    }

    public static string ToString<T>( this List<T> a_list, string a_delimiter, string a_endDelimiter = null ) {
        var listStr = "";
        foreach ( var str in a_list )
            listStr += $"{str}{a_delimiter}";

        listStr = listStr.Substring( 0, listStr.Length - 2 );
        if ( a_endDelimiter != null )
            listStr += a_endDelimiter;
        return listStr;
    }

    // List<MonoBehaviour>

    public static string ToString( this List<MonoBehaviour> a_list, string a_delimiter, string a_endDelimiter = null ) {
        var strList = new List<string>();
        foreach ( var o in a_list )
            strList.Add( o.name );

        return strList.ToString();
    }
}
