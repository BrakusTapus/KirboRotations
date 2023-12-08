using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using KirboRotations.Utility.Core;
using KirboRotations.Utility.ImGuiEx;
using KirboRotations.Utility.Logging;
using Lumina.Excel.GeneratedSheets;

namespace KirboRotations.Utility.GameAssists;

public static unsafe class GenericAssists
{

    public static void Log(this Exception e)
    {
        KirboLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Print<T>(this IEnumerable<T> x, string separator = ", ")
    {
        return x.Select((T x) => x.ToString()).Join(separator);
    }

    public static void ShellStart(string s)
    {
        Safe(delegate
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = s,
                UseShellExecute = true
            });
        }, (e) =>
        {
            Notify.Error($"Could not open {s.Cut(60)}\n{e}");
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Cut(this string s, int num)
    {
        if (s.Length <= num) return s;
        return s[0..num] + "...";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Repeat(this string s, int num)
    {
        StringBuilder str = new();
        for (var i = 0; i < num; i++)
        {
            str.Append(s);
        }
        return str.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join(this IEnumerable<string> e, string separator)
    {
        return string.Join(separator, e);
    }

    public static void Safe(System.Action a, Action<string> fail, bool suppressErrors = false)
    {
        try
        {
            a();
        }
        catch (Exception e)
        {
            try
            {
                fail(e.Message);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error("Error while trying to process error handler:");
                Serilog.Log.Error($"{ex.Message}\n{ex.StackTrace ?? ""}");
                suppressErrors = false;
            }
            if (!suppressErrors) Serilog.Log.Error($"{e.Message}\n{e.StackTrace ?? ""}");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Safe(System.Action a, bool suppressErrors = false)
    {
        try
        {
            a();
        }
        catch (Exception ex)
        {
            if (!suppressErrors)
            {
                Serilog.Log.Error(ex.Message + "\n" + ex.StackTrace);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Default(this string s, string defaultValue)
    {
        if (string.IsNullOrEmpty(s))
        {
            return defaultValue;
        }

        return s;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsIgnoreCase(this string s, string other)
    {
        return s.Equals(other, StringComparison.OrdinalIgnoreCase);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NullWhenEmpty(this string s)
    {
        if (!(s == string.Empty))
        {
            return s;
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty(this string s)
    {
        return string.IsNullOrEmpty(s);
    }

    public static IEnumerable<R> SelectMulti<T, R>(this IEnumerable<T> values, params Func<T, R>[] funcs)
    {
        foreach (T v in values)
        {
            foreach (Func<T, R> func in funcs)
            {
                yield return func(v);
            }
        }
    }
    public static bool TryGetFirst<K>(this IEnumerable<K> enumerable, Func<K, bool> predicate, out K value)
    {
        try
        {
            value = enumerable.First(predicate);
            return true;
        }
        catch (Exception)
        {
            value = default(K);
            return false;
        }
    }

    public static bool TryGetWorldByName(string world, out World worldId)
    {
        if (KirboSvc.Data.GetExcelSheet<World>().TryGetFirst((World x) => x.Name.ToString().Equals(world, StringComparison.OrdinalIgnoreCase), out var value))
        {
            worldId = value;
            return true;
        }

        worldId = null;
        return false;
    }

    public static Vector4 Invert(this Vector4 v)
    {
        Vector4 result = v;
        result.X = 1f - v.X;
        result.Y = 1f - v.Y;
        result.Z = 1f - v.Z;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToUint(this Vector4 color)
    {
        return ImGui.ColorConvertFloat4ToU32(color);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4 ToVector4(this uint color)
    {
        return ImGui.ColorConvertU32ToFloat4(color);
    }

    public static ref int ValidateRange(this ref int i, int min, int max)
    {
        if (i > max)
        {
            i = max;
        }

        if (i < min)
        {
            i = min;
        }

        return ref i;
    }

    public static ref float ValidateRange(this ref float i, float min, float max)
    {
        if (i > max)
        {
            i = max;
        }

        if (i < min)
        {
            i = min;
        }

        return ref i;
    }

}
