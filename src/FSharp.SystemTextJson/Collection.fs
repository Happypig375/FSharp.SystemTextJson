﻿namespace System.Text.Json.Serialization

open System
open System.Text.Json

type JsonListConverter<'T>() =
    inherit JsonConverter<list<'T>>()

    override __.Read(reader, _typeToConvert, options) =
        JsonSerializer.Deserialize<'T[]>(&reader, options)
        |> List.ofArray

    override __.Write(writer, value, options) =
        JsonSerializer.Serialize<seq<'T>>(writer, value, options)

type JsonListConverter() =
    inherit JsonConverterFactory()

    static member internal CanConvert(typeToConvert: Type) =
        TypeCache.isList typeToConvert

    static member internal CreateConverter(typeToConvert: Type) =
        typedefof<JsonListConverter<_>>
            .MakeGenericType([|typeToConvert.GetGenericArguments().[0]|])
            .GetConstructor([||])
            .Invoke([||])
        :?> JsonConverter

    override __.CanConvert(typeToConvert) =
        JsonListConverter.CanConvert(typeToConvert)

    override __.CreateConverter(typeToConvert, _options) =
        JsonListConverter.CreateConverter(typeToConvert)

type JsonSetConverter<'T when 'T : comparison>() =
    inherit JsonConverter<Set<'T>>()

    let rec read (acc: Set<'T>) (reader: byref<Utf8JsonReader>) options =
        if not (reader.Read()) then acc else
        match reader.TokenType with
        | JsonTokenType.EndArray -> acc
        | _ ->
            let elt = JsonSerializer.Deserialize<'T>(&reader, options)
            read (Set.add elt acc) &reader options

    override __.Read(reader, typeToConvert, options) =
        if reader.TokenType <> JsonTokenType.StartArray then
            raise (JsonException("Failed to parse type " + typeToConvert.FullName + ", expected JSON array, found " + string reader.TokenType))
        read Set.empty &reader options

    override __.Write(writer, value, options) =
        JsonSerializer.Serialize<seq<'T>>(writer, value, options)

type JsonSetConverter() =
    inherit JsonConverterFactory()

    static member internal CanConvert(typeToConvert: Type) =
        TypeCache.isSet typeToConvert

    static member internal CreateConverter(typeToConvert: Type) =
        typedefof<JsonSetConverter<_>>
            .MakeGenericType([|typeToConvert.GetGenericArguments().[0]|])
            .GetConstructor([||])
            .Invoke([||])
        :?> JsonConverter

    override __.CanConvert(typeToConvert) =
        JsonSetConverter.CanConvert(typeToConvert)

    override __.CreateConverter(typeToConvert, _options) =
        JsonSetConverter.CreateConverter(typeToConvert)