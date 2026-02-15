using System.Globalization;

internal static class Program
{
    private const int inn = 100;
    private const int imm = 150;

    private static int n = 0, m = 0;
    private static readonly int[] nn = new int[inn], nk = new int[inn];
    private static readonly int[,] nm = new int[3, imm], nm1 = new int[3, imm];
    private static readonly double[] unom = new double[inn], p0 = new double[inn], q0 = new double[inn], g = new double[inn], b = new double[inn];
    private static readonly double[] r = new double[imm], x = new double[imm], gy = new double[imm], by = new double[imm], kt = new double[imm];
    private static readonly double[] gg = new double[inn], bb = new double[inn], va = new double[inn], vr = new double[inn];
    private static readonly double[] gr = new double[imm], bx = new double[imm];
    private static readonly double[] p = new double[inn], q = new double[inn], ja = new double[inn], jr = new double[inn];
    private static readonly double[] ds = new double[2 * inn];
    private static readonly double[,] A = new double[2 * inn, 2 * inn];

    private static int Iteraz = 10;
    private static double Precision = 0.001;
    private static readonly int[] nus = new int[10];
    private const int kkk = 100, kk = 10;

    private static readonly CultureInfo C = CultureInfo.InvariantCulture;

    // Для разделения потерь
    private static readonly double[] rd = new double[30], dP = new double[30], ta = new double[30], tr = new double[30], tza = new double[20], tzr = new double[20];
    private static readonly double[,] aa = new double[30, 20], a = new double[30, 20], ar = new double[30, 20];
    private static readonly int[] na = new int[30], ka = new int[30], nr = new int[30], kr = new int[30], N = new int[30], k = new int[30];

    private static int Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo.CurrentCulture = C;
        CultureInfo.CurrentUICulture = C;

        Console.WriteLine("=== Расчёт установившегося режима (потокораспределение) ===");
        Console.Write("Введите путь к файлу .CDU: ");
        string inFile = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Задайте точность (например 0.001): ");
        Precision = ReadDouble(0.001);

        Console.Write("Задайте максимальное число итераций: ");
        Iteraz = ReadInt(10);

        if (!Preparation(inFile))
        {
            Console.WriteLine("Ошибка подготовки данных – расчёт невозможен.");
            return 1;
        }

        ZeroValue();
        Ylfl();

        using var fout = new StreamWriter("network.out", false);
        fout.WriteLine($"Число узлов n = {n}\tЧисло Ветвей m = {m}");
        fout.WriteLine("     Входные данные для расчета потокораспределения");
        fout.WriteLine("           У з л ы    с е т и ");
        fout.WriteLine(" Узел   Тип  Uном        P        Q        g           b ");

        for (int i = 0; i <= n; i++)
        {
            fout.WriteLine($"{nn[i],5}{nk[i],5}{Flex(unom[i], 0),7}{Flex(p0[i], 0),9}{Flex(q0[i], 0),9}{Flex(g[i], 5),9}{Flex(b[i], 5),12}");
        }

        fout.WriteLine();
        fout.WriteLine("               В е т в и    с е т и");
        fout.WriteLine("   N1   N2        r         x            b           g       Kt ");

        for (int j = 1; j <= m; j++)
        {
            fout.WriteLine($"{nm[1, j],5}{nm[2, j],5}{Flex(r[j], 3),10}{Flex(x[j], 2),10}{Flex(Math.Abs(by[j]), 7),14}{Flex(gy[j], 0),10}{Flex(kt[j], 0),9}");
        }

        fout.WriteLine();

        int count = 0;
        double norm = Func();
        while (norm > Precision && count < Iteraz)
        {
            Jacoby();
            Gauss();
            count++;
            norm = Func();
            fout.WriteLine($"{count}{Flex(norm, 6),12}");
        }

        if (count >= Iteraz && norm > Precision)
        {
            Console.WriteLine("Режим НЕ сошелся.");
        }
        else
        {
            Console.WriteLine($"Режим сошелся. Итерация={count}, невязка={Flex(norm,6)}");
            Load();
            CalculateLossDistribution();
        }

        Console.WriteLine("Файлы: network.cdu, network.err, network.out, network.rez, network.rip, network.tok, losses.rez");
        return 0;
    }

    private static double ReadDouble(double fallback)
    {
        string? s = Console.ReadLine();
        if (double.TryParse(s, NumberStyles.Float, C, out double v)) return v;
        s = s?.Replace(',', '.');
        return double.TryParse(s, NumberStyles.Float, C, out v) ? v : fallback;
    }

    private static int ReadInt(int fallback)
    {
        return int.TryParse(Console.ReadLine(), out int v) ? v : fallback;
    }

    private static void Raion(int nnn)
    {
        int kratn = nnn / kkk;
        while (kratn > 9) kratn /= kk;
        nus[kratn]++;
    }

    private static bool Preparation(string fname)
    {
        if (!File.Exists(fname)) return false;

        Array.Clear(nus);
        int j = 0, br = 0;

        foreach (var raw in File.ReadLines(fname))
        {
            var line = raw.Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;
            var t = line.Split((char[])null!, StringSplitOptions.RemoveEmptyEntries);
            if (t.Length < 2) continue;
            if (!int.TryParse(t[0], NumberStyles.Integer, C, out int code)) continue;

            if (code == 102 || code == 0102)
            {
                if (t.Length < 6) continue;
                nn[0] = ParseI(t[2]);
                unom[0] = ParseD(t[3]);
                p0[0] = ParseD(t[4]);
                q0[0] = ParseD(t[5]);
                nk[0] = 3;
                g[0] = b[0] = 0;
                Raion(nn[0]);
            }
            else if (code == 201 || code == 0201)
            {
                if (t.Length < 6) continue;
                j++;
                nn[j] = ParseI(t[2]);
                unom[j] = ParseD(t[3]);
                p0[j] = ParseD(t[4]);
                q0[j] = ParseD(t[5]);
                nk[j] = 1;
                g[j] = b[j] = 0;
                if (t.Length > 8)
                {
                    double uv = ParseD(t[8]);
                    if (uv > 0.1)
                    {
                        unom[j] = uv;
                        nk[j] = 2;
                    }
                }
                Raion(nn[j]);
            }
            else if (code == 301 || code == 0301)
            {
                if (t.Length < 4) continue;
                br++;
                nm[1, br] = ParseI(t[2]);
                nm[2, br] = ParseI(t[3]);

                if (nm[2, br] == 0)
                {
                    g[br] = t.Length > 4 ? ParseD(t[4]) : 0;
                    b[br] = t.Length > 5 ? ParseD(t[5]) : 0;
                    for (int l = 1; l <= j; l++)
                    {
                        if (nn[l] == nm[1, br])
                        {
                            b[l] = -b[br];
                            b[br] = 0;
                        }
                    }
                    br--;
                }
                else
                {
                    if (t.Length < 7) { br--; continue; }
                    r[br] = ParseD(t[4]);
                    x[br] = ParseD(t[5]);
                    by[br] = -ParseD(t[6]);
                    kt[br] = t.Length > 7 ? ParseD(t[7]) : 0;
                    if (Math.Abs(x[br]) < 1.001) x[br] = 1.01;
                    if (kt[br] < 0.001) kt[br] = 1;
                    gy[br] = 0;
                }
            }
        }

        n = j;
        m = br;

        using (var ferr = new StreamWriter("network.err", false))
        {
            bool ok = n > 0 && m > 0;

            for (int i = 1; i <= n; i++)
            {
                bool linked = false;
                for (int b1 = 1; b1 <= m; b1++)
                {
                    if (nn[i] == nm[1, b1] || nn[i] == nm[2, b1]) { linked = true; break; }
                }
                if (!linked)
                {
                    ok = false;
                    ferr.WriteLine($"Нет ветвей для узла {nn[i]}");
                }
            }

            for (int b1 = 1; b1 <= m; b1++)
            {
                bool n1 = false, n2 = false;
                for (int i = 0; i <= n; i++)
                {
                    if (nm[1, b1] == nn[i]) n1 = true;
                    if (nm[2, b1] == nn[i]) n2 = true;
                }
                if (!(n1 && n2))
                {
                    ok = false;
                    ferr.WriteLine($"Ошибка связи ветви {nm[1, b1]}-{nm[2, b1]}");
                }
            }

            if (!ok) return false;
        }

        for (int i = 0; i <= n; i++)
        {
            g[i] *= 1e-6;
            b[i] *= 1e-6;
        }
        for (int i = 1; i <= m; i++)
        {
            gy[i] *= 1e-6;
            by[i] *= 1e-6;
        }

        for (int i = 1; i <= m; i++)
        {
            for (int j1 = 0; j1 <= n; j1++)
            {
                if (nm[1, i] == nn[j1]) nm1[1, i] = j1;
                if (nm[2, i] == nn[j1]) nm1[2, i] = j1;
            }
        }

        File.Copy(fname, "network.cdu", true);
        return true;
    }

    private static int ParseI(string s) => int.Parse(s.Replace(',', '.'), NumberStyles.Integer, C);
    private static double ParseD(string s) => double.Parse(s.Replace(',', '.'), NumberStyles.Float, C);

    private static void ZeroValue()
    {
        for (int i = 0; i <= n; i++)
        {
            va[i] = unom[i];
            vr[i] = 0;
        }
    }

    private static void Ylfl()
    {
        for (int i = 1; i <= m; i++)
        {
            double c = r[i] * r[i] + x[i] * x[i];
            gr[i] = r[i] / c;
            bx[i] = -x[i] / c;
        }

        for (int i = 0; i <= n; i++)
        {
            gg[i] = g[i];
            bb[i] = b[i];
        }

        for (int j = 1; j <= m; j++)
        {
            int i1 = nm1[1, j], i2 = nm1[2, j];
            gg[i1] += gr[j] / (kt[j] * kt[j]) + gy[j] / 2.0;
            bb[i1] += bx[j] / (kt[j] * kt[j]) + by[j] / 2.0;
            gg[i2] += gr[j] + gy[j] / 2.0;
            bb[i2] += bx[j] + by[j] / 2.0;
        }
    }

    private static double Func()
    {
        double w = 0;

        for (int i = 0; i <= n; i++)
        {
            ja[i] = gg[i] * va[i] - bb[i] * vr[i];
            jr[i] = bb[i] * va[i] + gg[i] * vr[i];
        }

        for (int j = 1; j <= m; j++)
        {
            int i1 = nm1[1, j], i2 = nm1[2, j];
            ja[i1] -= (gr[j] * va[i2] - bx[j] * vr[i2]) / kt[j];
            jr[i1] -= (bx[j] * va[i2] + gr[j] * vr[i2]) / kt[j];
            ja[i2] -= (gr[j] * va[i1] - bx[j] * vr[i1]) / kt[j];
            jr[i2] -= (bx[j] * va[i1] + gr[j] * vr[i1]) / kt[j];
        }

        for (int i = 0; i <= n; i++)
        {
            p[i] = va[i] * ja[i] + vr[i] * jr[i];
            q[i] = vr[i] * ja[i] - va[i] * jr[i];
        }

        for (int i = 1; i <= n; i++)
        {
            ds[2 * i] = -(p[i] + p0[i]);
            double h;
            if (nk[i] == 1)
            {
                ds[2 * i - 1] = -(q[i] + q0[i]);
                h = Math.Abs(q0[i]) < 1 ? ds[2 * i - 1] : ds[2 * i - 1] / q0[i];
            }
            else
            {
                ds[2 * i - 1] = -(va[i] * va[i] + vr[i] * vr[i] - unom[i] * unom[i]) / unom[i];
                h = ds[2 * i - 1] / unom[i];
            }

            double pp = Math.Abs(p0[i]) < 1 ? ds[2 * i] : ds[2 * i] / p0[i];
            w += pp * pp + h * h;
        }

        return Math.Sqrt(w / (2 * n));
    }

    private static void Jacoby()
    {
        Array.Clear(A);

        for (int i = 1; i <= n; i++)
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

        for (int j = 1; j <= m; j++)
        {
            int i1 = nm1[1, j], i2 = nm1[2, j];
            if (i1 == 0 || i2 == 0) continue;

            int j1 = 2 * i1 - 1, j2 = 2 * i1, j3 = 2 * i2 - 1, j4 = 2 * i2;
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

    private static void Gauss()
    {
        var u = new double[2 * inn];

        for (int i = 1; i <= 2 * n - 1; i++)
        {
            double c = Math.Abs(A[i, i]) < 1e-7 ? 1e-7 : A[i, i];
            for (int j = i + 1; j <= 2 * n; j++)
                if (Math.Abs(A[i, j]) > 1e-7) A[i, j] /= c;

            ds[i] /= c;

            for (int k1 = i + 1; k1 <= 2 * n; k1++)
            {
                double d = A[k1, i];
                if (Math.Abs(d) <= 1e-7) continue;
                for (int l = i + 1; l <= 2 * n; l++)
                    if (Math.Abs(A[i, l]) > 1e-7) A[k1, l] -= A[i, l] * d;
                ds[k1] -= ds[i] * d;
            }
        }

        u[2 * n] = ds[2 * n] / A[2 * n, 2 * n];
        for (int k1 = 2 * n - 1; k1 >= 1; k1--)
        {
            double s = 0;
            for (int j = k1 + 1; j <= 2 * n; j++)
                if (Math.Abs(A[k1, j]) > 1e-7) s += A[k1, j] * u[j];
            u[k1] = ds[k1] - s;
        }

        for (int i = 1; i <= n; i++)
        {
            va[i] += u[2 * i - 1];
            vr[i] += u[2 * i];
        }
    }

    private static bool Load()
    {
        using var net2 = new StreamWriter("network.rez", false);
        using var tok = new StreamWriter("network.tok", false);
        using var raipot = new StreamWriter("network.rip", false);

        net2.WriteLine("          Результаты расчета по узлам");
        net2.WriteLine("    N        V         dV         P         Q        Pg       Qb");

        double sp = 0, sq = 0, spg = 0, sqb = 0, dPsum = 0;

        var saldo = new double[2, 10];
        var sumpot = new double[10];
        var sv = new int[10];
        var line = new double[10, 10];
        var linc = new int[10, 10];

        for (int i = 0; i <= n; i++)
        {
            double mv = va[i] * va[i] + vr[i] * vr[i];
            double dv = Math.Atan2(vr[i], va[i]) * 57.295779515;
            double pg = mv * g[i];
            double qb = -mv * b[i];
            sp += p[i]; sq += q[i]; spg += pg; sqb += qb;
            mv = Math.Sqrt(mv);

            net2.WriteLine($"{nn[i],5}{F2(mv),10}{F2(dv),10}{F2(-p[i]),10}{F2(-q[i]),10}{F2(pg),10}{F2(qb),10}");
        }

        net2.WriteLine("---------------------------------------------------");
        net2.WriteLine($"Баланс пассивных элементов {F2(sp),10}{F2(sq),10}{F2(spg),10}{F2(sqb),10}");
        net2.WriteLine("                         + потребление, - генерация ");
        net2.WriteLine();
        net2.WriteLine("                   Результаты расчета по ветвям");
        net2.WriteLine("   N1   N1       P12       Q12       P21       Q21       dP");

        for (int j = 1; j <= m; j++)
        {
            int i1 = nm1[1, j], i2 = nm1[2, j];

            double i1a = (va[i1] * gr[j] - vr[i1] * bx[j]) / (kt[j] * kt[j]) - (va[i2] * gr[j] - vr[i2] * bx[j]) / kt[j];
            double i1r = (va[i1] * bx[j] + vr[i1] * gr[j]) / (kt[j] * kt[j]) - (va[i2] * bx[j] + vr[i2] * gr[j]) / kt[j];
            double i2a = (va[i1] * gr[j] - vr[i1] * bx[j]) / kt[j] - (va[i2] * gr[j] - vr[i2] * bx[j]);
            double i2r = (va[i1] * bx[j] + vr[i1] * gr[j]) / kt[j] - (va[i2] * bx[j] + vr[i2] * gr[j]);

            double mv = va[i1] * va[i1] + vr[i1] * vr[i1];
            double p12 = va[i1] * i1a + vr[i1] * i1r - gy[j] * mv / 2.0;
            double q12 = vr[i1] * i1a - va[i1] * i1r - by[j] * mv / 2.0;

            mv = va[i2] * va[i2] + vr[i2] * vr[i2];
            double p21 = va[i2] * i2a + vr[i2] * i2r + gy[j] * mv / 2.0;
            double q21 = vr[i2] * i2a - va[i2] * i2r + by[j] * mv / 2.0;

            double dpl = Math.Abs(p21 - p12);
            dPsum += dpl;

            // В эталоне tok — внутренние индексы узлов
            tok.WriteLine($"{i1,5}{i2,5}{F4(i1a),10}{F4(i1r),10}{Flex(Math.Abs(r[j]),3),15}");

            net2.WriteLine($"{nn[i1],5}{nn[i2],5}{F2(-p12),10}{F2(-q12),10}{F2(p21),10}{F2(q21),10}{F2(dpl),10}");

            int rk = nn[i1] / kkk; while (rk > 9) rk /= kk;
            int r2 = nn[i2] / kkk; while (r2 > 9) r2 /= kk;

            if (rk == r2)
            {
                sv[rk]++;
                sumpot[rk] += dpl;
                if (Math.Abs(kt[j] - 1) < 0.001)
                {
                    int cls = 0;
                    if (unom[i1] <= 8) cls = 0;
                    else if (unom[i1] <= 15) cls = 1;
                    else if (unom[i1] <= 28) cls = 2;
                    else if (unom[i1] <= 70) cls = 3;
                    else if (unom[i1] <= 140) cls = 4;
                    else if (unom[i1] <= 270) cls = 5;
                    else if (unom[i1] <= 430) cls = 6;
                    else if (unom[i1] <= 600) cls = 7;
                    else if (unom[i1] <= 900) cls = 8;
                    else cls = 9;
                    line[rk, cls] += dpl;
                    linc[rk, cls]++;
                }
            }
            else
            {
                saldo[0, rk] += -p12;
                saldo[1, rk] += -q12;
                saldo[0, r2] += p21;
                saldo[1, r2] += q21;
            }
        }

        net2.WriteLine($"{F2(dPsum),60}");

        int[] ul = { 6, 10, 20, 35, 110, 220, 330, 500, 750, 1150 };
        for (int i = 0; i < 10; i++)
        {
            if (sv[i] == 0) continue;
            raipot.WriteLine($"Район  №{i,6}");
            for (int g1 = 0; g1 < 10; g1++)
            {
                if (linc[i, g1] != 0)
                {
                    raipot.WriteLine(" Потери в линиях");
                    raipot.WriteLine($"{ul[g1],5} кВ{Flex(line[i, g1],3),15}");
                }
            }
        }

        raipot.WriteLine();
        raipot.WriteLine("№ района          Сальдо P           Сальдо Q       Сумм. потери в районе");
        double sumoll = 0;
        for (int i = 0; i < 10; i++)
        {
            if (nus[i] == 0) continue;
            raipot.WriteLine($"{i,5}{F2(saldo[0, i]),20}{F2(saldo[1, i]),20}{F2(sumpot[i]),20}");
            sumoll += sumpot[i];
        }
        raipot.WriteLine($" Суммарные потери в сетях районов {F2(sumoll),25}");

        return true;
    }

    private static void Alpha()
    {
        const int maxDepth = 32;
        const double minCoef = 1e-10;

        var src = new double[30, 20];
        for (int i = 1; i <= m; i++)
            for (int j = 0; j <= n; j++)
                src[i, j] = aa[i, j];

        // result starts from direct coefficients
        var result = new double[30, 20];
        for (int i = 1; i <= m; i++)
            for (int j = 0; j <= n; j++)
                result[i, j] = src[i, j];

        for (int startBranch = 1; startBranch <= m; startBranch++)
        {
            var stack = new Stack<(int node, double coef, int depth)>();

            for (int j = 0; j <= n; j++)
            {
                if (Math.Abs(src[startBranch, j]) > minCoef)
                    stack.Push((j, src[startBranch, j], 0));
            }

            while (stack.Count > 0)
            {
                var (node, coef, depth) = stack.Pop();
                if (depth >= maxDepth || Math.Abs(coef) < minCoef) continue;

                for (int br = 1; br <= m; br++)
                {
                    if (N[br] != node) continue;
                    int nextNode = k[br];
                    double step = src[br, nextNode];
                    if (Math.Abs(step) < minCoef) continue;

                    double nextCoef = coef * step;
                    if (Math.Abs(nextCoef) < minCoef) continue;

                    result[startBranch, nextNode] += nextCoef;
                    stack.Push((nextNode, nextCoef, depth + 1));
                }
            }
        }

        for (int i = 1; i <= m; i++)
            for (int j = 0; j <= n; j++)
                aa[i, j] = result[i, j];
    }

    private static void CalculateLossDistribution()
    {
        if (!File.Exists("network.tok")) return;

        int L = m;
        int idx = 0;
        foreach (var line in File.ReadLines("network.tok"))
        {
            var t = line.Split((char[])null!, StringSplitOptions.RemoveEmptyEntries);
            if (t.Length < 5) continue;
            idx++;
            if (idx > L) break;

            // network.tok хранит внутренние индексы (как в эталоне)
            na[idx] = ParseI(t[0]);
            ka[idx] = ParseI(t[1]);
            ta[idx] = ParseD(t[2]);
            tr[idx] = ParseD(t[3]);
            rd[idx] = ParseD(t[4]);
            nr[idx] = na[idx];
            kr[idx] = ka[idx];
        }

        for (int j = 0; j <= n; j++) { tza[j] = 0; tzr[j] = 0; }
        for (int i = 1; i <= L; i++)
        {
            tza[na[i]] -= ta[i];
            tzr[na[i]] -= tr[i];
            tza[ka[i]] += ta[i];
            tzr[ka[i]] += tr[i];
        }

        using var net2 = new StreamWriter("losses.rez", false);
        net2.WriteLine("  Разделение потерь мощности электрической сети  ");
        net2.WriteLine("  Входные данные для расчета ");
        net2.WriteLine($"  Число узлов  = {n + 1}\tЧисло Ветвей  = {L}");
        net2.WriteLine("         Токи ветвей ");
        net2.WriteLine(" Ветвь Нач.   Кон.    Ток Ak   Ток Re    R  ");
        for (int i = 1; i <= L; i++) net2.WriteLine($"{nn[na[i]],8}{nn[ka[i]],8}{F2(ta[i]),10}{F2(tr[i]),10}{F2(Math.Abs(rd[i])),10}");

        net2.WriteLine("       Задающие токи узлов  ");
        net2.WriteLine("     Узел            ТЗа     ТЗр  ");
        for (int j = 1; j <= n; j++) net2.WriteLine($"{nn[j],8}{F2(tza[j]),10}{F2(tzr[j]),10}");
        net2.WriteLine($"{nn[0],8}{F2(tza[0]),10}{F2(tzr[0]),10}");

        for (int i = 1; i <= L; i++)
            for (int j = 0; j <= n; j++)
                aa[i, j] = a[i, j] = ar[i, j] = 0;

        for (int j = 0; j <= n; j++)
        {
            double sia = tza[j];
            for (int i = 1; i <= L; i++) if (na[i] == j) sia += ta[i];
            if (Math.Abs(sia) > 1e-10)
                for (int i = 1; i <= L; i++) if (ka[i] == j) a[i, j] = ta[i] / sia;
        }

        for (int j = 0; j <= n; j++)
        {
            double sia = tzr[j];
            for (int i = 1; i <= L; i++) if (nr[i] == j) sia += tr[i];
            if (Math.Abs(sia) > 1e-10)
                for (int i = 1; i <= L; i++) if (kr[i] == j) ar[i, j] = tr[i] / sia;
        }

        for (int i = 1; i <= L; i++) { N[i] = na[i]; k[i] = ka[i]; for (int j = 0; j <= n; j++) aa[i, j] = a[i, j]; }
        Alpha();
        for (int i = 1; i <= L; i++) for (int j = 0; j <= n; j++) a[i, j] = aa[i, j];

        for (int i = 1; i <= L; i++) { N[i] = nr[i]; k[i] = kr[i]; for (int j = 0; j <= n; j++) aa[i, j] = ar[i, j]; }
        Alpha();
        for (int i = 1; i <= L; i++) for (int j = 0; j <= n; j++) ar[i, j] = aa[i, j];

        for (int i = 1; i <= L; i++)
            for (int j = 0; j <= n; j++)
            {
                a[i, j] = tza[j] * a[i, j];
                ar[i, j] = tzr[j] * ar[i, j];
            }

        for (int i = 1; i <= L; i++) dP[i] = Math.Abs(rd[i]) * (ta[i] * ta[i] + tr[i] * tr[i]);

        net2.WriteLine();
        net2.WriteLine("              Результаты расчетов  ");
        net2.WriteLine();
        net2.WriteLine("          Составляющие потерь от нагрузок узлов   ");
        net2.Write("Ветвь    ");
        for (int j = 1; j <= n; j++) net2.Write($"{nn[j],6}");
        net2.WriteLine($"{nn[0],6}   dP");

        for (int i = 1; i <= L; i++)
        {
            net2.Write($"{nn[na[i]],4} {nn[ka[i]],4}");
            double total = 0;
            for (int j = 1; j <= n; j++)
            {
                double comp = 0;
                for (int kk1 = 0; kk1 <= n; kk1++) comp += a[i, j] * a[i, kk1] + ar[i, j] * ar[i, kk1];
                aa[i, j] = Math.Abs(rd[i]) * comp;
                total += aa[i, j];
                net2.Write($"{F2(aa[i, j]),6}");
            }
            double bal = 0;
            for (int kk1 = 0; kk1 <= n; kk1++) bal += a[i, 0] * a[i, kk1] + ar[i, 0] * ar[i, kk1];
            aa[i, 0] = Math.Abs(rd[i]) * bal;
            total += aa[i, 0];
            net2.WriteLine($"{F2(aa[i, 0]),6}{F2(total),7}");
        }

        net2.WriteLine();
        for (int i = 1; i <= L; i++)
        {
            net2.WriteLine($"Ветвь   {nn[na[i]]}-{nn[ka[i]]}  Потери  {F2(dP[i])}");
            net2.Write("Составляющие ");
            for (int j = 1; j <= n; j++) if (Math.Abs(aa[i, j]) > 1e-6) net2.Write($" dP{nn[j]}={F2(aa[i, j])}");
            if (Math.Abs(aa[i, 0]) > 1e-6) net2.Write($" dP{nn[0]}={F2(aa[i, 0])}");
            net2.WriteLine();
        }

        net2.WriteLine();
        net2.WriteLine("          Доля транзитных потерь от нагрузок узлов   ");
        net2.Write("Узлы    ");
        for (int j = 1; j <= n; j++) net2.Write($"{nn[j],6}");
        net2.WriteLine($"{nn[0],6}");
        net2.Write("Потери ");
        for (int j = 1; j <= n; j++)
        {
            double nodeLoss = 0;
            for (int i = 1; i <= L; i++) nodeLoss += aa[i, j];
            net2.Write($"{F2(nodeLoss),6}");
        }
        double balLoss = 0;
        for (int i = 1; i <= L; i++) balLoss += aa[i, 0];
        net2.WriteLine($"{F2(balLoss),6}");
    }
    private static string F2(double v) => Math.Round(v, 2, MidpointRounding.AwayFromZero).ToString("0.00", C);
    private static string F4(double v) => Math.Round(v, 4, MidpointRounding.AwayFromZero).ToString("0.0000", C);
    private static string Flex(double v, int maxDecimals = 7)
    {
        double rv = Math.Round(v, maxDecimals, MidpointRounding.AwayFromZero);
        if (Math.Abs(rv) < 5 * Math.Pow(10, -maxDecimals)) rv = 0;
        return rv.ToString("0." + new string('#', maxDecimals), C);
    }

}
