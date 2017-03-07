### Irony Portable Readme

The Irony Portable version does not support AST nor does it support any Dynamic Runtime code. This is due to a limitiation in *most* PCL targets where the *System.Reflection.Emit* namespace is not availablie (mainly *Xamarin.IOS*)

Therefore it is a highly cut down version of the original **Irony.Net** application.