using System.Globalization;

internal static class Program
{
    const int inn = 100;
    const int imm = 150;

    static int n = 0, m = 0;
    static readonly int[] nn = new int[inn], nk = new int[inn];
    static readonly int[,] nm = new int[3, imm], nm1 = new int[3, imm];
    static readonly double[] unom = new double[inn], p0 = new double[inn], q0 = new double[inn], g = new double[inn], b = new double[inn];
    static readonly double[] r = new double[imm], x = new double[imm], gy = new double[imm], by = new double[imm], kt = new double[imm];
    static readonly double[] gg = new double[inn], bb = new double[inn], va = new double[inn], vr = new double[inn];
    static readonly double[] gr = new double[imm], bx = new double[imm];
    static readonly double[] p = new double[inn], q = new double[inn], ja = new double[inn], jr = new double[inn];
    static readonly double[] ds = new double[2 * inn], nus = new double[10];
    static readonly double[,] A = new double[2 * inn, 2 * inn];

    static readonly double[] rd = new double[30], dP = new double[30], ta = new double[30], tr = new double[30], tza = new double[20], tzr = new double[20];
    static readonly double[,] aa = new double[30, 20], a = new double[30, 20], ar = new double[30, 20];
    static readonly int[] na = new int[30], ka = new int[30], nr = new int[30], kr = new int[30], N = new int[30], k = new int[30];

    static int Iteraz = 10;
    static double Precision = 0.001;
    static int kkk = 100, kk = 10;

    static readonly CultureInfo C = CultureInfo.InvariantCulture;

    static int Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Расчёт установившегося режима (C#) ===");

        Console.Write("Введите путь к исходному файлу .CDU: ");
        var inFile = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Введите точность (например 0.001): ");
        Precision = double.TryParse(Console.ReadLine(), NumberStyles.Float, C, out var pVal) ? pVal : 0.001;
        Console.Write("Введите максимальное число итераций: ");
        Iteraz = int.TryParse(Console.ReadLine(), out var it) ? it : 10;

        if (!Preparation(inFile))
        {
            Console.WriteLine("Ошибка подготовки. Проверьте файл network.err");
            return 1;
        }

        ZeroValue();
        Ylfl();

        using var outFile = new StreamWriter("network.out", false);
        outFile.WriteLine($"Число узлов n = {n}\tЧисло Ветвей m = {m}");
        outFile.WriteLine("-- итерации --");

        var count = 0;
        var norm = Func();
        while (norm > Precision && count < Iteraz)
        {
            Jacoby();
            Gauss();
            count++;
            norm = Func();
            outFile.WriteLine($"{count,4}{norm,14:F6}");
            Console.WriteLine($"Итерация {count}: невязка={norm:F6}");
        }

        if (count >= Iteraz && norm > Precision)
        {
            Console.WriteLine("Режим НЕ сошелся за заданное число итераций.");
            outFile.WriteLine($"Не сошелся за {count} итераций. Невязка={norm:F6}");
        }
        else
        {
            Console.WriteLine($"Режим сошелся. Итерация={count}, невязка={norm:F6}");
            outFile.WriteLine($"Сошелся. Итерация={count}, невязка={norm:F6}");
            Load();
            CalculateLossDistribution();
        }

        Console.WriteLine("Сформированы файлы: network.cdu, network.err, network.out, network.rez, network.rip, network.tok, losses.rez");
        return 0;
    }

    static void Raion(int nnn)
    {
        var kratn = nnn / kkk;
        while (kratn > 9) kratn /= kk;
        nus[kratn]++;
    }

    static bool Preparation(string fname)
    {
        if (!File.Exists(fname)) return false;
        var lines = File.ReadAllLines(fname);

        var j = 0;
        var br = 0;
        foreach (var line in lines)
        {
            var t = line.Split((char[])null!, StringSplitOptions.RemoveEmptyEntries);
            if (t.Length == 0) continue;
            var code = int.Parse(t[0], C);

            if (code == 102 || code == 0102)
            {
                nn[0] = int.Parse(t[2], C); unom[0] = double.Parse(t[3], C); p0[0] = double.Parse(t[4], C); q0[0] = double.Parse(t[5], C);
                nk[0] = 3; g[0] = b[0] = 0; Raion(nn[0]);
            }
            else if (code == 201 || code == 0201)
            {
                ++j;
                nn[j] = int.Parse(t[2], C); unom[j] = double.Parse(t[3], C); p0[j] = double.Parse(t[4], C); q0[j] = double.Parse(t[5], C);
                nk[j] = 1; g[j] = b[j] = 0;
                if (t.Length > 8 && double.TryParse(t[8], NumberStyles.Float, C, out var uv) && uv > 0.1) { unom[j] = uv; nk[j] = 2; }
                Raion(nn[j]);
            }
            else if (code == 301 || code == 0301)
            {
                ++br;
                nm[1, br] = int.Parse(t[2], C); nm[2, br] = int.Parse(t[3], C);
                if (nm[2, br] == 0)
                {
                    var gv = t.Length > 4 ? double.Parse(t[4], C) : 0;
                    var bv = t.Length > 5 ? double.Parse(t[5], C) : 0;
                    for (var l = 1; l <= j; l++) if (nn[l] == nm[1, br]) { b[l] = -bv; }
                    br--;
                }
                else
                {
                    r[br] = double.Parse(t[4], C); x[br] = double.Parse(t[5], C); by[br] = -double.Parse(t[6], C);
                    kt[br] = (t.Length > 7 ? double.Parse(t[7], C) : 0);
                    if (Math.Abs(x[br]) < 1.001) x[br] = 1.01;
                    if (kt[br] < 0.001) kt[br] = 1.0;
                    gy[br] = 0;
                }
            }
        }

        n = j; m = br;

        using var ferr = new StreamWriter("network.err", false);
        var ok = n > 0 && m > 0;

        for (var i = 1; i <= n; i++)
        {
            var linked = false;
            for (var b1 = 1; b1 <= m; b1++) if (nn[i] == nm[1, b1] || nn[i] == nm[2, b1]) linked = true;
            if (!linked) { ok = false; ferr.WriteLine($"Нет ветвей для узла {nn[i]}"); }
        }

        for (var i = 0; i <= n; i++) { g[i] *= 1e-6; b[i] *= 1e-6; }
        for (var i = 1; i <= m; i++) { gy[i] *= 1e-6; by[i] *= 1e-6; }

        for (var i = 1; i <= m; i++)
        {
            for (var j1 = 0; j1 <= n; j1++)
            {
                if (nm[1, i] == nn[j1]) nm1[1, i] = j1;
                if (nm[2, i] == nn[j1]) nm1[2, i] = j1;
            }
        }

        File.Copy(fname, "network.cdu", true);
        return ok;
    }

    static void ZeroValue() { for (var i = 0; i <= n; i++) { va[i] = unom[i]; vr[i] = 0; } }

    static void Ylfl()
    {
        for (var i = 1; i <= m; i++) { var c = r[i] * r[i] + x[i] * x[i]; gr[i] = r[i] / c; bx[i] = -x[i] / c; }
        for (var i = 0; i <= n; i++) { gg[i] = g[i]; bb[i] = b[i]; }
        for (var j = 1; j <= m; j++)
        {
            var i1 = nm1[1, j]; var i2 = nm1[2, j];
            gg[i1] += gr[j] / (kt[j] * kt[j]) + gy[j] / 2.0;
            bb[i1] += bx[j] / (kt[j] * kt[j]) + by[j] / 2.0;
            gg[i2] += gr[j] + gy[j] / 2.0;
            bb[i2] += bx[j] + by[j] / 2.0;
        }
    }

    static double Func()
    {
        double w = 0;
        for (var i = 0; i <= n; i++) { ja[i] = gg[i] * va[i] - bb[i] * vr[i]; jr[i] = bb[i] * va[i] + gg[i] * vr[i]; }
        for (var j = 1; j <= m; j++)
        {
            var i1 = nm1[1, j]; var i2 = nm1[2, j];
            ja[i1] -= (gr[j] * va[i2] - bx[j] * vr[i2]) / kt[j];
            jr[i1] -= (bx[j] * va[i2] + gr[j] * vr[i2]) / kt[j];
            ja[i2] -= (gr[j] * va[i1] - bx[j] * vr[i1]) / kt[j];
            jr[i2] -= (bx[j] * va[i1] + gr[j] * vr[i1]) / kt[j];
        }

        for (var i = 1; i <= n; i++)
        {
            p[i] = va[i] * ja[i] + vr[i] * jr[i];
            q[i] = vr[i] * ja[i] - va[i] * jr[i];
            ds[2 * i] = -(p[i] + p0[i]);
            double h;
            if (nk[i] == 1) { ds[2 * i - 1] = -(q[i] + q0[i]); h = Math.Abs(q0[i]) < 1 ? ds[2 * i - 1] : ds[2 * i - 1] / q0[i]; }
            else { ds[2 * i - 1] = -(va[i] * va[i] + vr[i] * vr[i] - unom[i] * unom[i]) / unom[i]; h = ds[2 * i - 1] / unom[i]; }
            var pp = Math.Abs(p0[i]) < 1 ? ds[2 * i] : ds[2 * i] / p0[i];
            w += pp * pp + h * h;
        }
        return Math.Sqrt(w / (2 * n));
    }

    static void Jacoby()
    {
        Array.Clear(A);
        for (var i = 1; i <= n; i++)
        {
            if (nk[i] == 1)
            {
                A[2 * i - 1, 2 * i - 1] = -bb[i] * va[i] + gg[i] * vr[i] - jr[i];
                A[2 * i - 1, 2 * i] = -gg[i] * va[i] - bb[i] * vr[i] + ja[i];
            }
            else
            {
                A[2 * i - 1, 2 * i - 1] = 2 * va[i] / unom[i];
                A[2 * i - 1, 2 * i] = 2 * vr[i] / unom[i];
            }
            A[2 * i, 2 * i - 1] = gg[i] * va[i] + bb[i] * vr[i] + ja[i];
            A[2 * i, 2 * i] = -bb[i] * va[i] + gg[i] * vr[i] + jr[i];
        }

        for (var j = 1; j <= m; j++)
        {
            var i1 = nm1[1, j]; var i2 = nm1[2, j];
            if (i1 == 0 || i2 == 0) continue;
            var j1 = 2 * i1 - 1; var j2 = 2 * i1; var j3 = 2 * i2 - 1; var j4 = 2 * i2;
            if (nk[i1] == 1)
            {
                A[j1, j3] = -(-bx[j] * va[i1] + gr[j] * vr[i1]) / kt[j];
                A[j1, j4] = -(-gr[j] * va[i1] - bx[j] * vr[i1]) / kt[j];
            }
            A[j2, j3] = -(gr[j] * va[i1] + bx[j] * vr[i1]) / kt[j];
            A[j2, j4] = -(-bx[j] * va[i1] + gr[j] * vr[i1]) / kt[j];
            if (nk[i2] == 1)
            {
                A[j3, j1] = -(-bx[j] * va[i2] + gr[j] * vr[i2]) / kt[j];
                A[j3, j2] = -(-gr[j] * va[i2] - bx[j] * vr[i2]) / kt[j];
            }
            A[j4, j1] = -(gr[j] * va[i2] + bx[j] * vr[i2]) / kt[j];
            A[j4, j2] = -(-bx[j] * va[i2] + gr[j] * vr[i2]) / kt[j];
        }
    }

    static void Gauss()
    {
        var u = new double[2 * inn];
        for (var i = 1; i <= 2 * n - 1; i++)
        {
            var c = Math.Abs(A[i, i]) < 1e-7 ? 1e-7 : A[i, i];
            for (var j = i + 1; j <= 2 * n; j++) if (Math.Abs(A[i, j]) > 1e-7) A[i, j] /= c;
            ds[i] /= c;
            for (var k1 = i + 1; k1 <= 2 * n; k1++)
            {
                var d = A[k1, i];
                if (Math.Abs(d) <= 1e-7) continue;
                for (var l = i + 1; l <= 2 * n; l++) if (Math.Abs(A[i, l]) > 1e-7) A[k1, l] -= A[i, l] * d;
                ds[k1] -= ds[i] * d;
            }
        }

        u[2 * n] = ds[2 * n] / A[2 * n, 2 * n];
        for (var k1 = 2 * n - 1; k1 >= 1; k1--)
        {
            double s = 0;
            for (var j = k1 + 1; j <= 2 * n; j++) if (Math.Abs(A[k1, j]) > 1e-7) s += A[k1, j] * u[j];
            u[k1] = ds[k1] - s;
        }
        for (var i = 1; i <= n; i++) { va[i] += u[2 * i - 1]; vr[i] += u[2 * i]; }
    }

    static void Load()
    {
        using var net2 = new StreamWriter("network.rez", false);
        using var tok = new StreamWriter("network.tok", false);
        using var rip = new StreamWriter("network.rip", false);

        net2.WriteLine("Результаты расчета по узлам");
        tok.WriteLine("N1 N2 Ia Ir R");
        var dPsum = 0.0;

        for (var j = 1; j <= m; j++)
        {
            var i1 = nm1[1, j]; var i2 = nm1[2, j];
            var i1a = (va[i1] * gr[j] - vr[i1] * bx[j]) / (kt[j] * kt[j]) - (va[i2] * gr[j] - vr[i2] * bx[j]) / kt[j];
            var i1r = (va[i1] * bx[j] + vr[i1] * gr[j]) / (kt[j] * kt[j]) - (va[i2] * bx[j] + vr[i2] * gr[j]) / kt[j];
            var i2a = (va[i1] * gr[j] - vr[i1] * bx[j]) / kt[j] - (va[i2] * gr[j] - vr[i2] * bx[j]);
            var i2r = (va[i1] * bx[j] + vr[i1] * gr[j]) / kt[j] - (va[i2] * bx[j] + vr[i2] * gr[j]);

            var mv = va[i1] * va[i1] + vr[i1] * vr[i1];
            var p12 = va[i1] * i1a + vr[i1] * i1r - gy[j] * mv / 2.0;
            var q12 = vr[i1] * i1a - va[i1] * i1r - by[j] * mv / 2.0;
            mv = va[i2] * va[i2] + vr[i2] * vr[i2];
            var p21 = va[i2] * i2a + vr[i2] * i2r + gy[j] * mv / 2.0;
            var q21 = vr[i2] * i2a - va[i2] * i2r + by[j] * mv / 2.0;
            var dPbr = Math.Abs(p21 - p12);
            dPsum += dPbr;

            tok.WriteLine($"{nn[i1],5}{nn[i2],5}{i1a,10:F4}{i1r,10:F4}{r[j],10:F3}");
            net2.WriteLine($"{nn[i1],5}{nn[i2],5}{-p12,10:F3}{-q12,10:F3}{p21,10:F3}{q21,10:F3}{dPbr,10:F3}");
        }
        net2.WriteLine($"Суммарные потери: {dPsum:F3}");
        rip.WriteLine("Файл network.rip сформирован.");
    }

    static void Alpha()
    {
        const int maxDepth = 10000;
        for (var start = 1; start <= m; start++)
        {
            var currentNode = k[start];
            var coef = aa[start, currentNode];
            for (var depth = 1; depth <= maxDepth; depth++)
            {
                var found = false;
                for (var target = 1; target <= m; target++)
                {
                    if (N[target] != currentNode) continue;
                    currentNode = k[target];
                    coef *= aa[target, currentNode];
                    aa[start, currentNode] += coef;
                    found = true;
                    break;
                }
                if (!found || Math.Abs(coef) < 1e-10) break;
            }
        }
    }

    static void CalculateLossDistribution()
    {
        var lines = File.ReadAllLines("network.tok");
        var L = m;
        for (var i = 1; i <= L && i < lines.Length; i++)
        {
            var t = lines[i].Split((char[])null!, StringSplitOptions.RemoveEmptyEntries);
            if (t.Length < 5) continue;
            var node1 = int.Parse(t[0], C); var node2 = int.Parse(t[1], C);
            ta[i] = double.Parse(t[2], C); tr[i] = double.Parse(t[3], C); rd[i] = double.Parse(t[4], C);
            na[i] = Array.IndexOf(nn, node1, 0, n + 1);
            ka[i] = Array.IndexOf(nn, node2, 0, n + 1);
            nr[i] = na[i]; kr[i] = ka[i];
        }

        for (var j = 0; j <= n; j++) { tza[j] = 0; tzr[j] = 0; }
        for (var i = 1; i <= L; i++) { tza[na[i]] -= ta[i]; tzr[na[i]] -= tr[i]; tza[ka[i]] += ta[i]; tzr[ka[i]] += tr[i]; }

        for (var j = 0; j <= n; j++)
        {
            var sia = tza[j];
            for (var i = 1; i <= L; i++) if (na[i] == j) sia += ta[i];
            if (Math.Abs(sia) > 1e-10) for (var i = 1; i <= L; i++) if (ka[i] == j) a[i, j] = ta[i] / sia;

            sia = tzr[j];
            for (var i = 1; i <= L; i++) if (nr[i] == j) sia += tr[i];
            if (Math.Abs(sia) > 1e-10) for (var i = 1; i <= L; i++) if (kr[i] == j) ar[i, j] = tr[i] / sia;
        }

        for (var i = 1; i <= L; i++) { N[i] = na[i]; k[i] = ka[i]; for (var j = 0; j <= n; j++) aa[i, j] = a[i, j]; }
        Alpha();
        for (var i = 1; i <= L; i++) for (var j = 0; j <= n; j++) a[i, j] = aa[i, j];

        for (var i = 1; i <= L; i++) { N[i] = nr[i]; k[i] = kr[i]; for (var j = 0; j <= n; j++) aa[i, j] = ar[i, j]; }
        Alpha();
        for (var i = 1; i <= L; i++) for (var j = 0; j <= n; j++) ar[i, j] = aa[i, j];

        using var loss = new StreamWriter("losses.rez", false);
        loss.WriteLine("Разделение потерь мощности");
        for (var i = 1; i <= L; i++) dP[i] = rd[i] * (ta[i] * ta[i] + tr[i] * tr[i]);
        for (var i = 1; i <= L; i++)
        {
            double total = 0;
            for (var j = 0; j <= n; j++)
            {
                a[i, j] = tza[j] * a[i, j]; ar[i, j] = tzr[j] * ar[i, j];
                var comp = 0.0;
                for (var kk1 = 0; kk1 <= n; kk1++) comp += a[i, j] * a[i, kk1] + ar[i, j] * ar[i, kk1];
                aa[i, j] = rd[i] * comp;
                total += aa[i, j];
            }
            loss.WriteLine($"Ветвь {nn[na[i]]}-{nn[ka[i]]}: dP={dP[i]:F4}, сумма компонент={total:F4}");
        }
    }
}
