// Dibuja el video del portal con la forma del cristal del espejo.
// La transparencia sale del sprite del portal (_MainTex); el color, del video (_VideoTex).
// El cristal es un paralelogramo isométrico: el video se inclina con él para seguir la pared.
Shader "Carencia/PortalVideo"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite (máscara)", 2D) = "white" {}
        _VideoTex ("Video", 2D) = "black" {}
        // x = u izquierda, y = u derecha, z = v inferior en la esquina izquierda, w = alto en v
        _Cristal ("Cristal en el sprite", Vector) = (0, 1, 0, 1)
        // Cuánto baja el borde inferior por cada unidad de u (inclinación isométrica)
        _Inclinacion ("Inclinación", Float) = 0
        // xy = escala del video dentro del cristal, zw = desplazamiento (0.5 = centrado)
        _Encuadre ("Encuadre del video", Vector) = (1, 1, 0.5, 0.5)
        _Brillo ("Brillo", Float) = 1
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" "CanUseSpriteAtlas" = "True" "RenderPipeline" = "UniversalPipeline" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_VideoTex);
            SAMPLER(sampler_VideoTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Cristal;
                float _Inclinacion;
                float4 _Encuadre;
                float _Brillo;
            CBUFFER_END

            Varyings vert(Attributes entrada)
            {
                Varyings salida;
                salida.positionCS = TransformObjectToHClip(entrada.positionOS.xyz);
                salida.color = entrada.color;
                salida.uv = entrada.uv;
                return salida;
            }

            half4 frag(Varyings entrada) : SV_Target
            {
                half mascara = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, entrada.uv).a;

                // Coordenadas dentro del cristal: s a lo ancho, t a lo alto siguiendo la inclinación
                float s = (entrada.uv.x - _Cristal.x) / (_Cristal.y - _Cristal.x);
                float baseV = _Cristal.z + _Inclinacion * (entrada.uv.x - _Cristal.x);
                float t = (entrada.uv.y - baseV) / _Cristal.w;
                float2 video = (float2(s, t) - 0.5) * _Encuadre.xy + _Encuadre.zw;

                half3 color = SAMPLE_TEXTURE2D(_VideoTex, sampler_VideoTex, saturate(video)).rgb * _Brillo;
                return half4(color * entrada.color.rgb, mascara * entrada.color.a);
            }
            ENDHLSL
        }
    }
}
