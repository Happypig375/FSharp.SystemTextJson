﻿namespace System.Text.Json.Serialization

open System
open System.Runtime.InteropServices

type JsonFSharpConverter
    (
        [<Optional; DefaultParameterValue(JsonUnionEncoding.Default)>]
        unionEncoding: JsonUnionEncoding
    ) =
    inherit JsonConverterFactory()

    override this.CanConvert(typeToConvert) =
        JsonListConverter.CanConvert(typeToConvert) ||
        JsonRecordConverter.CanConvert(typeToConvert) ||
        JsonUnionConverter.CanConvert(typeToConvert)

    static member internal CreateConverter(typeToConvert, unionEncoding) =
        if JsonListConverter.CanConvert(typeToConvert) then
            JsonListConverter.CreateConverter(typeToConvert)
        elif JsonRecordConverter.CanConvert(typeToConvert) then
            JsonRecordConverter.CreateConverter(typeToConvert)
        elif JsonUnionConverter.CanConvert(typeToConvert) then
            JsonUnionConverter.CreateConverter(typeToConvert, unionEncoding)
        else
            invalidOp ("Not an F# record or union type: " + typeToConvert.FullName)

    override this.CreateConverter(typeToConvert, _options) =
        JsonFSharpConverter.CreateConverter(typeToConvert, unionEncoding)

[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Struct)>]
type JsonFSharpConverterAttribute
    (
        [<Optional; DefaultParameterValue(JsonUnionEncoding.Default)>]
        unionEncoding: JsonUnionEncoding
    ) =
    inherit JsonConverterAttribute()

    override __.CreateConverter(typeToConvert) =
        JsonFSharpConverter.CreateConverter(typeToConvert, unionEncoding)
