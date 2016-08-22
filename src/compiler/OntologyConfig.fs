﻿module compiler.OntologyConfig

type PublishItem = {
    Uri: string
    Label: string
    Required: bool
    Format: string
    OutFormatMask: string
    PropertyPath: string list
}

type ConfigItem = {
    Schema: string
    JsonLD: string
    Map: bool
    Publish: PublishItem list

}

type OntologyConfig = {
    SchemaBase: string
    UrlBase: string
    QSBase: string
    ThingBase: string
    IndexName: string
    TypeName: string
    SchemaDetails: ConfigItem list
}