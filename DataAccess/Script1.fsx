#r "System.Xml"
#r "System.Xml.Linq"
#r "System.Configuration"
#load @"d:\Projects\Ashlynn\DataAccess\Config.fs"
#load @"d:\Projects\Ashlynn\DataAccess\Utils.fs"
#load @"d:\Projects\Ashlynn\DataAccess\Entry.fs"

open Ashlynn.DataAccess.Config
open Ashlynn.DataAccess.Utils
open Ashlynn.DataAccess.Entry
open System
open System.Configuration

ConfigurationManager.AppSettings.["BaseDirectory"] <- @"d:\temp"

Ashlynn.DataAccess.Entry.save (DateTime.Today) "dfafd"
