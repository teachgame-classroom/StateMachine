// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33535,y:32712,varname:node_9361,prsc:2|normal-6167-RGB,emission-2460-OUT,custl-5085-OUT,alpha-1965-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:8068,x:30954,y:32455,varname:node_8068,prsc:2;n:type:ShaderForge.SFN_LightColor,id:3406,x:32896,y:32953,varname:node_3406,prsc:2;n:type:ShaderForge.SFN_LightVector,id:6869,x:30596,y:32644,varname:node_6869,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:9684,x:30596,y:32772,prsc:2,pt:True;n:type:ShaderForge.SFN_HalfVector,id:9471,x:30596,y:32923,varname:node_9471,prsc:2;n:type:ShaderForge.SFN_Dot,id:7782,x:30808,y:32687,cmnt:Lambert,varname:node_7782,prsc:2,dt:1|A-6869-OUT,B-9684-OUT;n:type:ShaderForge.SFN_Dot,id:3269,x:30808,y:32887,cmnt:Blinn-Phong,varname:node_3269,prsc:2,dt:1|A-9684-OUT,B-9471-OUT;n:type:ShaderForge.SFN_Multiply,id:2746,x:32561,y:32847,cmnt:Specular Contribution,varname:node_2746,prsc:2|A-9479-OUT,B-8546-OUT;n:type:ShaderForge.SFN_Tex2d,id:851,x:31795,y:32221,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:1,isnm:False;n:type:ShaderForge.SFN_Exp,id:1700,x:30808,y:33044,varname:node_1700,prsc:2,et:1|IN-9978-OUT;n:type:ShaderForge.SFN_Slider,id:5328,x:29738,y:33133,ptovrint:False,ptlb:SpecularPower,ptin:_SpecularPower,varname:_SpecularPower,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Power,id:5267,x:30976,y:32962,varname:node_5267,prsc:2|VAL-3269-OUT,EXP-1700-OUT;n:type:ShaderForge.SFN_Add,id:2159,x:32896,y:32827,cmnt:Combine,varname:node_2159,prsc:2|A-1261-OUT,B-2746-OUT,C-9765-OUT;n:type:ShaderForge.SFN_Multiply,id:5085,x:33123,y:32921,cmnt:Attenuate and Color,varname:node_5085,prsc:2|A-2159-OUT,B-3406-RGB;n:type:ShaderForge.SFN_ConstantLerp,id:9978,x:30596,y:33046,varname:node_9978,prsc:2,a:11,b:1|IN-1647-OUT;n:type:ShaderForge.SFN_Color,id:4865,x:31623,y:32484,ptovrint:False,ptlb:SpecularColor,ptin:_SpecularColor,varname:_SpecularColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_AmbientLight,id:7528,x:32856,y:32657,varname:node_7528,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2460,x:33032,y:32596,cmnt:Ambient Light,varname:node_2460,prsc:2|A-851-RGB,B-7528-RGB;n:type:ShaderForge.SFN_Tex2d,id:5361,x:31655,y:32693,varname:_rampedShade,prsc:2,tex:21e468148a9c121448010cdcde81e60a,ntxv:1,isnm:False|UVIN-4575-OUT,TEX-8821-TEX;n:type:ShaderForge.SFN_Append,id:5796,x:31168,y:32736,varname:node_5796,prsc:2|A-7782-OUT,B-5703-OUT;n:type:ShaderForge.SFN_Vector1,id:5703,x:30951,y:32784,varname:node_5703,prsc:2,v1:0;n:type:ShaderForge.SFN_Tex2d,id:6029,x:32277,y:32104,ptovrint:False,ptlb:ShadowColorMap,ptin:_ShadowColorMap,varname:_ShadowColorMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Lerp,id:1261,x:32595,y:32589,varname:node_1261,prsc:2|A-6029-RGB,B-851-RGB,T-3662-OUT;n:type:ShaderForge.SFN_Tex2d,id:6167,x:33307,y:32472,ptovrint:True,ptlb:NormalMap,ptin:_BumpTex,varname:_BumpTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Fresnel,id:4892,x:31807,y:33455,varname:node_4892,prsc:2|EXP-4951-OUT;n:type:ShaderForge.SFN_Slider,id:4665,x:31273,y:33462,ptovrint:False,ptlb:LimLightWidth,ptin:_LimLightWidth,varname:_LimLightWidth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Subtract,id:4659,x:31996,y:33498,varname:node_4659,prsc:2|A-4892-OUT,B-3140-OUT;n:type:ShaderForge.SFN_Slider,id:8615,x:31347,y:33618,ptovrint:False,ptlb:LimLightPower,ptin:_LimLightPower,varname:_LimLightPower,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Tex2dAsset,id:8821,x:31251,y:32306,ptovrint:False,ptlb:ToonRamp,ptin:_ToonRamp,varname:_ToonRamp,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:21e468148a9c121448010cdcde81e60a,ntxv:1,isnm:False;n:type:ShaderForge.SFN_RemapRange,id:2253,x:31859,y:32693,varname:node_2253,prsc:2,frmn:0,frmx:1,tomn:0,tomx:2|IN-5361-RGB;n:type:ShaderForge.SFN_Tex2d,id:6177,x:31655,y:32928,varname:_rampedSpec,prsc:2,tex:21e468148a9c121448010cdcde81e60a,ntxv:0,isnm:False|UVIN-9928-OUT,TEX-8821-TEX;n:type:ShaderForge.SFN_Append,id:9928,x:31350,y:32948,varname:_baseSpec,prsc:2|A-3533-OUT,B-5703-OUT;n:type:ShaderForge.SFN_RemapRange,id:7056,x:31845,y:32927,varname:node_7056,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-6177-RGB;n:type:ShaderForge.SFN_Clamp01,id:9479,x:32271,y:32926,varname:node_9479,prsc:2|IN-7056-OUT;n:type:ShaderForge.SFN_Clamp01,id:3662,x:32043,y:32693,varname:_shadeRemap,prsc:2|IN-2253-OUT;n:type:ShaderForge.SFN_Multiply,id:484,x:32194,y:33316,varname:node_484,prsc:2|A-9534-OUT,B-4659-OUT;n:type:ShaderForge.SFN_Add,id:9534,x:32110,y:33130,varname:node_9534,prsc:2|A-3662-OUT,B-1463-OUT;n:type:ShaderForge.SFN_Vector1,id:1463,x:31926,y:33154,varname:node_1463,prsc:2,v1:0.5;n:type:ShaderForge.SFN_RemapRange,id:4951,x:31617,y:33437,varname:node_4951,prsc:2,frmn:1,frmx:0,tomn:1,tomx:11|IN-4665-OUT;n:type:ShaderForge.SFN_RemapRange,id:3140,x:31747,y:33618,varname:node_3140,prsc:2,frmn:0,frmx:1,tomn:1,tomx:0|IN-8615-OUT;n:type:ShaderForge.SFN_Clamp01,id:5774,x:32394,y:33135,varname:node_5774,prsc:2|IN-484-OUT;n:type:ShaderForge.SFN_Multiply,id:9765,x:32561,y:33008,cmnt:Specular Contribution,varname:node_9765,prsc:2|A-5774-OUT,B-8546-OUT,C-6241-OUT;n:type:ShaderForge.SFN_Multiply,id:4805,x:32136,y:32529,varname:node_4805,prsc:2|A-903-OUT,B-4865-RGB;n:type:ShaderForge.SFN_Relay,id:8546,x:32358,y:32732,varname:_specColor,prsc:2|IN-4805-OUT;n:type:ShaderForge.SFN_Append,id:1295,x:31168,y:32585,varname:node_1295,prsc:2|A-8068-OUT,B-5703-OUT;n:type:ShaderForge.SFN_Multiply,id:4575,x:31351,y:32689,varname:_baseaShade,prsc:2|A-5796-OUT,B-1295-OUT;n:type:ShaderForge.SFN_Tex2d,id:7794,x:30043,y:32879,ptovrint:False,ptlb:SpecularPowerMap,ptin:_SpecularPowerMap,varname:_SpecularPowerMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2401,x:30231,y:33046,varname:node_2401,prsc:2|A-7794-R,B-5328-OUT;n:type:ShaderForge.SFN_Clamp01,id:1647,x:30427,y:33046,varname:node_1647,prsc:2|IN-2401-OUT;n:type:ShaderForge.SFN_RemapRange,id:903,x:31979,y:32355,varname:node_903,prsc:2,frmn:0,frmx:1,tomn:0,tomx:4|IN-851-RGB;n:type:ShaderForge.SFN_Relay,id:9962,x:32336,y:31989,varname:node_9962,prsc:2|IN-851-A;n:type:ShaderForge.SFN_Relay,id:2526,x:33061,y:32402,varname:node_2526,prsc:2|IN-9962-OUT;n:type:ShaderForge.SFN_Relay,id:1965,x:33341,y:32814,varname:node_1965,prsc:2|IN-2526-OUT;n:type:ShaderForge.SFN_Multiply,id:3533,x:31168,y:32962,varname:node_3533,prsc:2|A-5267-OUT,B-8068-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:6241,x:32394,y:33262,varname:node_6241,prsc:2;proporder:851-6029-7794-5328-4865-8615-4665-6167-8821;pass:END;sub:END;*/

Shader "SaladMixStudio/ToonRamp_Transparent" {
    Properties {
        _MainTex ("MainTex", 2D) = "gray" {}
        _ShadowColorMap ("ShadowColorMap", 2D) = "black" {}
        _SpecularPowerMap ("SpecularPowerMap", 2D) = "white" {}
        _SpecularPower ("SpecularPower", Range(0, 1)) = 1
        _SpecularColor ("SpecularColor", Color) = (1,1,1,1)
        _LimLightPower ("LimLightPower", Range(0, 1)) = 1
        _LimLightWidth ("LimLightWidth", Range(0, 1)) = 0.5
        _BumpTex ("NormalMap", 2D) = "bump" {}
        _ToonRamp ("ToonRamp", 2D) = "gray" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _SpecularPower;
            uniform float4 _SpecularColor;
            uniform sampler2D _ShadowColorMap; uniform float4 _ShadowColorMap_ST;
            uniform sampler2D _BumpTex; uniform float4 _BumpTex_ST;
            uniform float _LimLightWidth;
            uniform float _LimLightPower;
            uniform sampler2D _ToonRamp; uniform float4 _ToonRamp_ST;
            uniform sampler2D _SpecularPowerMap; uniform float4 _SpecularPowerMap_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpTex_var = UnpackNormal(tex2D(_BumpTex,TRANSFORM_TEX(i.uv0, _BumpTex)));
                float3 normalLocal = _BumpTex_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = (_MainTex_var.rgb*UNITY_LIGHTMODEL_AMBIENT.rgb);
                float4 _ShadowColorMap_var = tex2D(_ShadowColorMap,TRANSFORM_TEX(i.uv0, _ShadowColorMap));
                float node_5703 = 0.0;
                float2 _baseaShade = (float2(max(0,dot(lightDirection,normalDirection)),node_5703)*float2(attenuation,node_5703));
                float4 _rampedShade = tex2D(_ToonRamp,TRANSFORM_TEX(_baseaShade, _ToonRamp));
                float3 _shadeRemap = saturate((_rampedShade.rgb*2.0+0.0));
                float4 _SpecularPowerMap_var = tex2D(_SpecularPowerMap,TRANSFORM_TEX(i.uv0, _SpecularPowerMap));
                float2 _baseSpec = float2((pow(max(0,dot(normalDirection,halfDirection)),exp2(lerp(11,1,saturate((_SpecularPowerMap_var.r*_SpecularPower)))))*attenuation),node_5703);
                float4 _rampedSpec = tex2D(_ToonRamp,TRANSFORM_TEX(_baseSpec, _ToonRamp));
                float3 _specColor = ((_MainTex_var.rgb*4.0+0.0)*_SpecularColor.rgb);
                float3 finalColor = emissive + ((lerp(_ShadowColorMap_var.rgb,_MainTex_var.rgb,_shadeRemap)+(saturate((_rampedSpec.rgb*2.0+-1.0))*_specColor)+(saturate(((_shadeRemap+0.5)*(pow(1.0-max(0,dot(normalDirection, viewDirection)),(_LimLightWidth*-10.0+11.0))-(_LimLightPower*-1.0+1.0))))*_specColor*attenuation))*_LightColor0.rgb);
                fixed4 finalRGBA = fixed4(finalColor,_MainTex_var.a);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _SpecularPower;
            uniform float4 _SpecularColor;
            uniform sampler2D _ShadowColorMap; uniform float4 _ShadowColorMap_ST;
            uniform sampler2D _BumpTex; uniform float4 _BumpTex_ST;
            uniform float _LimLightWidth;
            uniform float _LimLightPower;
            uniform sampler2D _ToonRamp; uniform float4 _ToonRamp_ST;
            uniform sampler2D _SpecularPowerMap; uniform float4 _SpecularPowerMap_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpTex_var = UnpackNormal(tex2D(_BumpTex,TRANSFORM_TEX(i.uv0, _BumpTex)));
                float3 normalLocal = _BumpTex_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float4 _ShadowColorMap_var = tex2D(_ShadowColorMap,TRANSFORM_TEX(i.uv0, _ShadowColorMap));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float node_5703 = 0.0;
                float2 _baseaShade = (float2(max(0,dot(lightDirection,normalDirection)),node_5703)*float2(attenuation,node_5703));
                float4 _rampedShade = tex2D(_ToonRamp,TRANSFORM_TEX(_baseaShade, _ToonRamp));
                float3 _shadeRemap = saturate((_rampedShade.rgb*2.0+0.0));
                float4 _SpecularPowerMap_var = tex2D(_SpecularPowerMap,TRANSFORM_TEX(i.uv0, _SpecularPowerMap));
                float2 _baseSpec = float2((pow(max(0,dot(normalDirection,halfDirection)),exp2(lerp(11,1,saturate((_SpecularPowerMap_var.r*_SpecularPower)))))*attenuation),node_5703);
                float4 _rampedSpec = tex2D(_ToonRamp,TRANSFORM_TEX(_baseSpec, _ToonRamp));
                float3 _specColor = ((_MainTex_var.rgb*4.0+0.0)*_SpecularColor.rgb);
                float3 finalColor = ((lerp(_ShadowColorMap_var.rgb,_MainTex_var.rgb,_shadeRemap)+(saturate((_rampedSpec.rgb*2.0+-1.0))*_specColor)+(saturate(((_shadeRemap+0.5)*(pow(1.0-max(0,dot(normalDirection, viewDirection)),(_LimLightWidth*-10.0+11.0))-(_LimLightPower*-1.0+1.0))))*_specColor*attenuation))*_LightColor0.rgb);
                fixed4 finalRGBA = fixed4(finalColor * _MainTex_var.a,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
