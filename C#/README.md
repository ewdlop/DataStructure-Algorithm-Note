# README

```csharp
  public static string FindCollectionName<T>(T type) =>
  type switch
  {
      _ when typeof(int).IsAssignableFrom(type.GetType()) => "Messages",
      _ when typeof(double).IsAssignableFrom(type.GetType()) => "UserData",
      _ when typeof(ICloneable).IsAssignableFrom(type.GetType()) => "UserData",
      _ => ""
  };
```
