Shader "FX/Surface Reflection"
{ 
    Properties
    {
        _MainAlpha("MainAlpha", Range(0, 1)) = 1
        _ReflectionAlpha("ReflectionAlpha", Range(0, 1)) = 1
        _TintColor ("Tint Color (RGB)", Color) = (1,1,1)
        _MainTex ("MainTex (RGBA)", 2D) = ""
        _ReflectionTex ("ReflectionTex", 2D) = "white" { TexGen ObjectLinear }
    }
 
    //Two texture cards: full thing
    Subshader
    { 
        Tags {Queue = Transparent}
        ZWrite Off
        Colormask RGBA
        Color [_TintColor]
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            SetTexture[_ReflectionTex] { constantColor(0,0,0, [_ReflectionAlpha]) matrix [_ProjMatrix] combine texture * previous, constant} 
        }
        Pass
        {
            SetTexture[_MainTex] { constantColor(0,0,0, [_MainAlpha]) combine texture * primary, texture * constant}
        }
    }
 
    //Fallback: just main texture
    Subshader
    {
        Pass
        {
            SetTexture [_MainTex] { combine texture }
        }
    }
}