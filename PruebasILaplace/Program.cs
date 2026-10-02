using System.Text.RegularExpressions;
Console.OutputEncoding = System.Text.Encoding.UTF8;

do
{
    Console.Clear();
    T("TRANSFORMADA DE LAPLACE");
    T("Ejemplos: 5, t, 3t^2, e^t, e^(2t). Para sumas usa espacios: 3 + t^2 + e^(2t)");
    string[] terminos = (Console.ReadLine() ?? "")
        .Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(p => p != "+").ToArray();

    if (terminos.Length == 0) { T("Debes escribir una función."); continue; }

    if (terminos.Length > 1)
    {
        T("Aplicamos linealidad: la transformada de una suma es la suma de transformadas.");
        E($"∫[0,∞] {string.Join(" + ", terminos.Select(t => $"e^(-st)({t})"))} dt");
    }

    var res = terminos.Select(Resolver).TakeWhile(r => r != "").ToList();
    if (res.Count == terminos.Length)
    {
        Console.WriteLine();
        T("RESULTADO FINAL");
        E($"L(f) = {string.Join(" + ", res.Select(r => res.Count > 1 ? $"({r})" : r))}");
    }
    T("\n¿Resolver otra función? (s/n)");
} while ((Console.ReadLine() ?? "").Trim().ToLower() == "s");

// ---------- Estilo de salida ----------
static void Color(string rgb, string s) => Console.WriteLine($"\u001b[38;2;{rgb}m{s}\u001b[0m");
static void T(string s) => Color("255;0;0", s);
static void E(string s) => Color("192;192;192", s);
static void Real() => T("Calculando");
static int Coef(string s) => s is "" or "+" ? 1 : s == "-" ? -1 : int.Parse(s);

// ---------- Resolver cada término ----------
static string Resolver(string p)
{
    Console.WriteLine();
    T($"--- Resolviendo: {p} ---");

    // Constante
    if (Regex.IsMatch(p, @"^[+-]?\d{1,9}$"))
    {
        int c = int.Parse(p);
        if (c == 0) { T("La transformada de cero es cero."); return "0"; }
        string k = c < 0 ? $"({c})" : $"{c}";

        T("Reescribir la integral impropia con un límite:");
        E($"{k} lim b->∞ ∫[0,b] e^(-st) dt");
        T("Integramos:");
        E($"lim b->∞ : [-{k}/s(e^(-st))][0,b]");
        T("Evaluacion...");
        E($"lim b->∞ : [-{k}/s(e^(-s*b))] - [-{k}/s(e^(-s*0))]");
        Real(); T("Para s > 0, e^(-sb) -> 0");
        E($"lim b->∞ : [0] - [-{k}/s(e^(-s*0))]");
        Real();
        E($"- [-{k}/s(1)] = {k}/s(1)");
        Real();
        E($"{c}/s");
        return $"{c}/s";
    }

    // t, t^2, 3t^2, -2t ...
    var m = Regex.Match(p, @"^(?<c>[+-]?\d{0,9})\*?t(?:\^(?<n>\d{1,2}))?$");
    if (m.Success)
    {
        int c = Coef(m.Groups["c"].Value);
        int n = m.Groups["n"].Success ? int.Parse(m.Groups["n"].Value) : 1;
        if (n > 20) { T("Usa una potencia entera entre 0 y 20."); return ""; }
        if (c == 0) { T("El término es cero. Su transformada es 0."); return "0"; }

        T("Reescribir la integral impropia con un límite:");
        E($"{c} lim b->∞ ∫[0,b] t^{n}e^(-st) dt   (s > 0)");
        if (n > 0) T("Integración por partes: ∫u dv = uv - ∫v du, con I_k(b) = ∫[0,b] t^k e^(-st) dt");
        for (int k = n; k >= 1; k--)
        {
            T($"\nPotencia {k}:");
            E($"u = t^{k}, du = {k}t^{k - 1} dt, dv = e^(-st) dt, v = -e^(-st)/s");
            E($"I_{k}(b) = [-t^{k}e^(-st)/s][0,b] + ({k}/s)I_{k - 1}(b)");
            T("Evaluacion"); E($"-b^{k}e^(-sb)/s -> 0 cuando b->∞ (s > 0)");
        }
        T("\nIntegral base:");
        E("I_0(b) = [-e^(-st)/s][0,b] = (1 - e^(-sb))/s -> 1/s");

        decimal fact = 1;
        for (int k = 1; k <= n; k++)
        {
            fact *= k; Real();
            E($"lim b->∞ I_{k}(b) = {fact}/s^{k + 1}");
        }
        string den = n == 0 ? "s" : $"s^{n + 1}";
        Real();
        E($"{c} * {fact}/{den} = {c * fact}/{den}");
        return $"{c * fact}/{den}";
    }

    // e^t, e^(2t), e^(-t), 3e^(2t) ...
    var e = Regex.Match(p, @"^(?<c>[+-]?\d{0,9})\*?e\^(?:t|\((?<a>[+-]?\d{0,9})t\))$");
    if (e.Success)
    {
        int c = Coef(e.Groups["c"].Value);
        int a = e.Groups["a"].Success ? Coef(e.Groups["a"].Value) : 1;
        if (c == 0) { T("El término es cero. Su transformada es 0."); return "0"; }
        string q = a > 0 ? $"(s - {a})" : a < 0 ? $"(s + {-(long)a})" : "s";

        T("Multiplicamos las exponenciales:");
        E($"e^(-st) * e^({a}t) = e^((-s + ({a}))t) = e^(-{q}t)");
        T("Reescribir la integral impropia con q = " + q + ":");
        E($"{c} lim b->∞ ∫[0,b] e^(-qt) dt");
        T("Sustitución: u = -qt, du = -q dt");
        E("∫e^(-qt) dt = -e^(-qt)/q");
        T("Evaluacion");
        E($"{c} lim b->∞ [-e^(-qb)/q + 1/q]");
        Real(); T("Si q > 0, e^(-qb) -> 0");
        E($"{c}/{q}");
        T($"Condición: s > {a}");
        return $"{c}/{q}";
    }

    T($"No se reconoce el término: {p}");
    T("válidos: 5, t, 3t^2, e^t, 2e^(-t).");
    return "";
}