#include <iostream>
#include <fstream>
#include <iomanip>
#include <cstring>
#include <cstdlib>
#include <cmath>
#include <vector>
#include <string>

using namespace std;

// Константы
const int inn = 100;          // макс. число узлов
const int imm = 150;          // макс. число ветвей

// Глобальные переменные
int n = 0, m = 0;             // число узлов и ветвей
int nn[inn], nk[inn];
int nm[3][imm], nm1[3][imm];
float unom[inn], p0[inn], q0[inn], g[inn], b[inn];
float r[imm], x[imm], gy[imm], by[imm], kt[imm];
float gg[inn], bb[inn], va[inn], vr[inn];
float gr[imm], bx[imm];
float p[inn], q[inn], ja[inn], jr[inn];
float ds[2 * inn], A[2 * inn][2 * inn];

int Iteraz = 10;              // по умолчанию
float Precision = 0.001f;     // по умолчанию
int kkk = 100, kk = 10;       // для определения района
int nus[10];                  // счетчик узлов по районам

// Переменные для разделения потерь
float rd[30], dP[30];
float ta[30], tr[30], tza[20], tzr[20];
float aa[30][20], a[30][20], ar[30][20], sia;
int na[30], ka[30], nr[30], kr[30], N[30], k[30], k0, kx;
int trn[10], nuzlN[10][13], nuzlK[10][13], linc[10][10], sv[10];
float saldo[2][10], sumpot[10], line[10][10], tran[10][13];

// Прототипы функций
bool preparation(const string& fname);
void zerovalue();
void ylfl();
float func(int count);
void jacoby();
void gauss();
bool load();
void raion(int nnn);
void alpha();
void calculateLossDistribution();

// Функция распределения узлов по районам
void raion(int nnn)
{
    int kratn = nnn / kkk;
    while (kratn > 9) kratn /= kk;
    nus[kratn]++;
}

// Функция alpha для разделения потерь
// Константы для настройки алгоритма
const int MAX_DEPTH = 99999999;        // максимальная глубина распространения вместо фиксированных 10
const float MIN_COEFFICIENT = 1e-10f; // минимальное значение коэффициента для остановки

// Улучшенная функция распространения коэффициентов
void alpha()
{
    // Временные переменные для читаемости
    int current_branch, target_branch;
    int current_node;
    float current_coefficient;

    // Обрабатываем каждую ветвь как начальную точку распространения
    for (int start_branch = 1; start_branch <= m; start_branch++)
    {
        current_branch = start_branch;
        current_node = k[current_branch];  // целевой узел текущей ветви
        current_coefficient = aa[start_branch][current_node];

        // Распространение по цепочке связей с контролем глубины
        for (int depth = 1; depth <= MAX_DEPTH; depth++)
        {
            bool chain_continued = false;

            // Ищем ветви, которые продолжают цепочку от текущего узла
            for (target_branch = 1; target_branch <= m; target_branch++)
            {
                // Если ветвь начинается в нашем целевом узле
                if (N[target_branch] == current_node)
                {
                    // Обновляем текущие значения для следующей итерации
                    current_branch = target_branch;
                    current_node = k[current_branch];
                    current_coefficient = current_coefficient * aa[target_branch][current_node];

                    // Накопление коэффициента в результирующей матрице
                    aa[start_branch][current_node] += current_coefficient;

                    chain_continued = true;
                    break; // переходим к следующему уровню глубины
                }
            }

            // Критерии остановки распространения:
            // 1. Не найдено продолжения цепочки
            // 2. Коэффициент стал слишком мал для учёта
            if (!chain_continued || fabs(current_coefficient) < MIN_COEFFICIENT)
            {
                break;
            }
        }

        // Дополнительная проверка на превышение глубины
        if (current_coefficient >= MIN_COEFFICIENT)
        {
            cout << "Предупреждение: для ветви " << start_branch
                << " не достигнута сходимость после " << MAX_DEPTH
                << " итераций. Последний коэффициент: " << current_coefficient << endl;
        }
    }
}

// Функция разделения потерь - ИСПРАВЛЕННАЯ
// Функция разделения потерь - ИСПРАВЛЕННАЯ ВЕРСИЯ
// Функция разделения потерь - ИСПРАВЛЕННАЯ ВЕРСИЯ
// Функция разделения потерь - ИСПРАВЛЕННАЯ ВЕРСИЯ
void calculateLossDistribution()
{
    // Инициализация массивов
    for (int i = 0; i < 10; i++) {
        saldo[0][i] = 0; sv[i] = 0; saldo[1][i] = 0; sumpot[i] = 0; trn[i] = 0;
        for (int b = 0; b < 10; b++) { linc[i][b] = 0; line[i][b] = 0; }
        for (int b = 0; b < 13; b++) { nuzlK[i][b] = 0; nuzlN[i][b] = 0; tran[i][b] = 0; }
    }

    int L = m;     // число ветвей

    // Чтение файла токов
    ifstream net1("network.tok");
    if (!net1) {
        cout << "Невозможно открыть файл network.tok" << endl;
        return;
    }

    // Пропускаем заголовок
    string header;
    getline(net1, header);

    // Чтение данных о токах - ВАЖНО: в файле ФАКТИЧЕСКИЕ номера узлов!
    cout << "--- ЧТЕНИЕ ФАЙЛА TOK ---" << endl;
    for (int i = 1; i <= L; i++) {
        int node1, node2;
        if (!(net1 >> node1 >> node2 >> ta[i] >> tr[i] >> rd[i])) {
            cout << "Ошибка чтения данных токов для ветви " << i << endl;
            break;
        }

        // Преобразуем ФАКТИЧЕСКИЕ номера узлов во внутренние индексы
        na[i] = -1;
        ka[i] = -1;

        for (int idx = 0; idx <= n; idx++) {
            if (node1 == nn[idx]) na[i] = idx;
            if (node2 == nn[idx]) ka[i] = idx;
        }

        // Проверяем, что узлы найдены
        if (na[i] == -1) {
            cout << "ОШИБКА: Узел " << node1 << " не найден!" << endl;
        }
        if (ka[i] == -1) {
            cout << "ОШИБКА: Узел " << node2 << " не найден!" << endl;
        }

        nr[i] = na[i];
        kr[i] = ka[i];

        cout << "Ветвь " << i << ": " << node1 << "(" << na[i] << ") -> "
            << node2 << "(" << ka[i] << ") I=(" << ta[i] << "," << tr[i]
            << ") R=" << rd[i] << endl;
    }
    net1.close();

    // ВЫЧИСЛЕНИЕ ЗАДАЮЩИХ ТОКОВ УЗЛОВ
    cout << "\n--- РАСЧЕТ ЗАДАЮЩИХ ТОКОВ ---" << endl;

    // Обнуляем массивы задающих токов
    for (int j = 0; j <= n; j++) {
        tza[j] = 0;
        tzr[j] = 0;
    }

    // Суммируем токи всех ветвей, подключенных к узлу
    for (int i = 1; i <= L; i++) {
        // Ток ВЫХОДИТ из начального узла - ОТРИЦАТЕЛЬНЫЙ знак
        tza[na[i]] = tza[na[i]] - ta[i];
        tzr[na[i]] = tzr[na[i]] - tr[i];

        // Ток ВХОДИТ в конечный узел - ПОЛОЖИТЕЛЬНЫЙ знак  
        tza[ka[i]] = tza[ka[i]] + ta[i];
        tzr[ka[i]] = tzr[ka[i]] + tr[i];
    }

    // ОТЛАДОЧНЫЙ ВЫВОД - проверяем баланс токов
    cout << "\n--- ПРОВЕРКА БАЛАНСА ТОКОВ ---" << endl;
    float sum_ta = 0, sum_tr = 0;
    for (int j = 0; j <= n; j++) {
        sum_ta += tza[j];
        sum_tr += tzr[j];
        cout << "Узел " << nn[j] << "(" << j << "): Tза=" << tza[j]
            << ", Tзр=" << tzr[j] << endl;
    }
    cout << "Сумма активных токов: " << sum_ta << " (должна быть близка к 0)" << endl;
    cout << "Сумма реактивных токов: " << sum_tr << " (должна быть близка к 0)" << endl;


    /*
    // Проверим конкретно для узла 13
    cout << "\n--- ПРОВЕРКА УЗЛА 13 ---" << endl;
    float sum_ta_without_13 = 0, sum_tr_without_13 = 0;
    for (int j = 1; j <= n; j++) {
        sum_ta_without_13 += tza[j];
        sum_tr_without_13 += tzr[j];
    }
    cout << "Сумма Tза всех узлов кроме 13: " << sum_ta_without_13 << endl;
    cout << "Сумма Tзр всех узлов кроме 13: " << sum_tr_without_13 << endl;
    cout << "Tза узла 13: " << tza[0] << " (должно быть ≈ " << -sum_ta_without_13 << ")" << endl;
    cout << "Tзр узла 13: " << tzr[0] << " (должно быть ≈ " << -sum_tr_without_13 << ")" << endl;
    */
    // Создание файла результатов разделения потерь
    ofstream net2("losses.rez");
    if (!net2) {
        cout << "Невозможно открыть файл losses.rez" << endl;
        return;
    }

    net2.setf(ios::fixed | ios::showpoint);
    net2 << "  Разделение потерь мощности электрической сети  \n";
    net2 << "  Входные данные для расчета \n";
    net2 << "  Число узлов  = " << (n + 1) << "\t" << "Число Ветвей  = " << L << endl;
    net2 << "         Токи ветвей \n ";
    net2 << " Ветвь Нач.   Кон.    Ток Ak   Ток Re    R  \n";

    for (int i = 1; i <= L; i++) {
        net2 << setw(5) << nn[na[i]]
            << setw(8) << nn[ka[i]]
            << setw(10) << setprecision(4) << ta[i]
            << setw(10) << setprecision(4) << tr[i]
            << setw(10) << setprecision(4) << rd[i] << endl;
    }

    net2 << "       Задающие токи узлов  \n";
    net2 << "     Узел            ТЗа     ТЗр  \n";

    // ВЫВОДИМ УЗЛЫ В ПРАВИЛЬНОМ ПОРЯДКЕ: сначала обычные узлы, потом балансирующий
    // Обычные узлы (индексы 1..n)
    for (int j = 1; j <= n; j++) {
        net2 << setw(8) << nn[j]
            << setw(12) << setprecision(4) << tza[j]
            << setw(12) << setprecision(4) << tzr[j] << endl;
    }
    // Балансирующий узел (индекс 0) - В КОНЦЕ
    net2 << setw(8) << nn[0]
        << setw(12) << setprecision(4) << tza[0]
        << setw(12) << setprecision(4) << tzr[0] << endl;

    // Инициализация матриц коэффициентов распределения
    for (int i = 1; i <= L; i++) {
        for (int j = 0; j <= n; j++) {
            aa[i][j] = 0;
            a[i][j] = 0;
            ar[i][j] = 0;
        }
    }

    // Расчет коэффициентов распределения для активной составляющей
    for (int j = 0; j <= n; j++) {
        sia = tza[j];

        // Суммируем токи, выходящие из узла
        for (int i = 1; i <= L; i++) {
            if (na[i] == j) {
                sia = sia + ta[i];
            }
        }

        // Распределяем токи, входящие в узел
        if (fabs(sia) > 1e-10) {
            for (int i = 1; i <= L; i++) {
                if (ka[i] == j) {
                    a[i][j] = ta[i] / sia;
                }
            }
        }
    }

    // Расчет коэффициентов распределения для реактивной составляющей
    for (int j = 0; j <= n; j++) {
        sia = tzr[j];

        for (int i = 1; i <= L; i++) {
            if (nr[i] == j) {
                sia = sia + tr[i];
            }
        }

        if (fabs(sia) > 1e-10) {
            for (int i = 1; i <= L; i++) {
                if (kr[i] == j) {
                    ar[i][j] = tr[i] / sia;
                }
            }
        }
    }

    // Расчет матрицы альфа для активной составляющей
    for (int i = 1; i <= L; i++) {
        N[i] = na[i];
        k[i] = ka[i];
        for (int j = 0; j <= n; j++) {
            aa[i][j] = a[i][j];
        }
    }
    alpha();

    // Перекачиваем обратно в a
    for (int i = 1; i <= L; i++) {
        for (int j = 0; j <= n; j++) {
            a[i][j] = aa[i][j];
        }
    }

    // Расчет матрицы альфа для реактивной составляющей
    for (int i = 1; i <= L; i++) {
        N[i] = nr[i];
        k[i] = kr[i];
        for (int j = 0; j <= n; j++) {
            aa[i][j] = ar[i][j];
        }
    }
    alpha();

    // Перекачиваем обратно в ar
    for (int i = 1; i <= L; i++) {
        for (int j = 0; j <= n; j++) {
            ar[i][j] = aa[i][j];
        }
    }

    // Пересчет матриц в действительные токи
    for (int i = 1; i <= L; i++) {
        for (int j = 0; j <= n; j++) {
            a[i][j] = tza[j] * a[i][j];
            ar[i][j] = tzr[j] * ar[i][j];
        }
    }

    // Вычисление токов через составляющие
    for (int i = 1; i <= L; i++) {
        for (int j = 0; j <= n; j++) {
            aa[i][j] = a[i][j] * a[i][j] + ar[i][j] * ar[i][j];
            aa[i][j] = sqrt(aa[i][j]);
        }
    }

    // Вычисление вектора потерь в ветвях
    for (int i = 1; i <= L; i++) {
        dP[i] = rd[i] * (ta[i] * ta[i] + tr[i] * tr[i]);
    }

    // Вывод составляющих потерь от нагрузок узлов
    net2 << "          Составляющие потерь от нагрузок узлов   \n";
    net2 << "Ветвь    ";
    // Заголовок - обычные узлы сначала, потом балансирующий
    for (int j = 1; j <= n; j++) {
        net2 << nn[j] << setw(8);
    }
    net2 << nn[0] << setw(8); // балансирующий узел в конце
    net2 << "   dP\n";

    for (int i = 1; i <= L; i++) {
        net2 << nn[na[i]] << "-" << nn[ka[i]];

        float total_loss = 0;
        // Обычные узлы сначала
        for (int j = 1; j <= n; j++) {
            // Расчет составляющей потерь от узла j в ветви i
            float loss_component = 0;
            for (int k = 0; k <= n; k++) {
                loss_component += a[i][j] * a[i][k] + ar[i][j] * ar[i][k];
            }
            aa[i][j] = rd[i] * loss_component;
            total_loss += aa[i][j];
            net2 << setw(8) << setprecision(4) << aa[i][j];
        }
        // Балансирующий узел в конце
        float balance_loss = 0;
        for (int k = 0; k <= n; k++) {
            balance_loss += a[i][0] * a[i][k] + ar[i][0] * ar[i][k];
        }
        aa[i][0] = rd[i] * balance_loss;
        total_loss += aa[i][0];
        net2 << setw(8) << setprecision(4) << aa[i][0];

        net2 << setw(10) << setprecision(4) << total_loss << endl;
    }

    net2 << endl;

    // Вывод результатов по ветвям
    for (int i = 1; i <= L; i++) {
        net2 << "Ветвь   " << nn[na[i]] << "-" << nn[ka[i]];
        net2 << "  Потери  " << dP[i] << endl;
        net2 << "Составляющие ";
        // Обычные узлы сначала
        for (int j = 1; j <= n; j++) {
            if (fabs(aa[i][j]) > 1e-6) {
                net2 << " dP" << nn[j] << "=" << aa[i][j];
            }
        }
        // Балансирующий узел в конце
        if (fabs(aa[i][0]) > 1e-6) {
            net2 << " dP" << nn[0] << "=" << aa[i][0];
        }
        net2 << endl;
    }

    net2 << endl;

    // Вывод доли транзитных потерь по узлам
    net2 << "          Доля транзитных потерь от нагрузок узлов   \n";
    net2 << "Узлы    ";
    // Обычные узлы сначала
    for (int j = 1; j <= n; j++) {
        net2 << nn[j] << setw(8);
    }
    net2 << nn[0] << setw(8); // балансирующий узел в конце
    net2 << endl;

    net2 << "Потери ";
    // Обычные узлы сначала
    for (int j = 1; j <= n; j++) {
        float node_loss = 0;
        for (int i = 1; i <= L; i++) {
            node_loss += aa[i][j];
        }
        net2 << setw(8) << setprecision(4) << node_loss;
    }
    // Балансирующий узел в конце
    float balance_total_loss = 0;
    for (int i = 1; i <= L; i++) {
        balance_total_loss += aa[i][0];
    }
    net2 << setw(8) << setprecision(4) << balance_total_loss;
    net2 << endl;

    net2.close();
    cout << "Результаты разделения потерь записаны в файл: losses.rez" << endl;
}

// Главная функция
int main()
{
    setlocale(LC_ALL, "ru");
    cout << "=== Расчёт установившегося режима (потокораспределение) ===\n\n";

    // Ввод точности
    cout << "Задайте точность (например 0.001): ";
    cin >> Precision;

    // Ввод максимального числа итераций
    cout << "Задайте максимальное число итераций: ";
    cin >> Iteraz;

    // Ввод пути к файлу
    string inFile;
    cout << "Введите путь к файлу с данными (формат txt): D:/network.cdu ";
    cin >> inFile;

    cout << "\n-- Чтение и подготовка данных --\n";
    cout << "Используется файл: " << inFile << "\n";

    // Подготовка данных
    if (!preparation(inFile))
    {
        cerr << "\nОшибка подготовки данных – расчёт невозможен.\n";
        return 1;
    }

    // Вывод исходных данных
    cout << "\n-- Исходные данные после чтения и преобразования --\n";
    cout << "Число узлов n = " << n << "  ветвей m = " << m << "\n";

    cout << fixed;
    cout << "Узлы:  №  Uном      P0     Q0       g          b\n";
    for (int i = 0; i <= n; ++i)
        cout << setw(4) << nn[i]
        << setw(8) << setprecision(1) << unom[i]
        << setw(8) << setprecision(2) << p0[i]
        << setw(8) << setprecision(2) << q0[i]
        << setw(10) << setprecision(6) << g[i]
        << setw(10) << setprecision(6) << b[i] << "\n";

    cout << "\nВетви: N1 N2      r        x         b         g       Kt\n";
    for (int j = 1; j <= m; ++j)
        cout << setw(4) << nm[1][j]
        << setw(4) << nm[2][j]
        << setw(10) << setprecision(4) << r[j]
        << setw(10) << setprecision(4) << x[j]
        << setw(10) << setprecision(6) << by[j]
        << setw(10) << setprecision(6) << gy[j]
        << setw(8) << setprecision(3) << kt[j] << "\n";

    // Инициализация и расчет
    zerovalue();
    ylfl();

    // Создание файла с исходными данными
    ofstream fout("network.out");
    if (!fout) {
        cerr << "Не могу создать network.out\n";
        return 1;
    }

    fout.setf(ios::fixed | ios::showpoint);

    fout << "Число узлов n = " << n << "\tЧисло Ветвей m = " << m << "\n";
    fout << "     Входные данные для расчета потокораспределения\n";
    fout << "           У з л ы    с е т и \n";
    fout << " Узел   Тип  Uном        P        Q        g           b \n";

    fout << fixed;
    for (int i = 0; i <= n; ++i)
        fout << setw(5) << nn[i] << setw(5) << nk[i]
        << setw(7) << setprecision(1) << unom[i]
        << setw(9) << setprecision(2) << p0[i]
        << setw(9) << setprecision(2) << q0[i]
        << setw(9) << setprecision(6) << g[i]
        << setw(12) << setprecision(6) << b[i] << '\n';

    fout << "\n               В е т в и    с е т и\n";
    fout << "   N1   N2        r         x            b           g       Kt \n";

    for (int j = 1; j <= m; ++j)
        fout << setw(5) << nm[1][j] << setw(5) << nm[2][j]
        << setw(10) << setprecision(4) << r[j]
        << setw(10) << setprecision(4) << x[j]
        << setw(14) << setprecision(6) << by[j]
        << setw(10) << setprecision(6) << gy[j]
        << setw(9) << setprecision(3) << kt[j] << '\n';

    fout << "\n-- итерации --\n";

    // Итерационный процесс
    int count = 0;
    float norm = func(count);
    cout << "\n-- Начало итераций --\n";
    cout << "Итерация " << count << "  невязка = " << setprecision(6) << norm << "\n";

    while (norm > Precision && count < Iteraz)
    {
        jacoby();
        gauss();
        ++count;
        norm = func(count);
        cout << "Итерация " << count << "  невязка = " << setprecision(6) << norm << "\n";
        fout << count << setw(12) << setprecision(6) << norm << endl;
    }

    if (count >= Iteraz && norm > Precision)
    {
        cout << "\nЗа заданное число итераций сходимость НЕ достигнута.\n";
        fout << "\nЗа " << count << " итераций сходимость не достигнута. Последняя невязка = " << norm << '\n';
    }
    else
    {
        cout << "\nРасчёт сошёлся на итерации " << count << "  невязка = " << setprecision(6) << norm << "\n";
        fout << "\nРасчёт сошёлся на итерации " << count << ", невязка = " << norm << '\n';
        load();   // формируем файлы отчётов

        // Отладочный вывод
        cout << "Проверка создания network.tok..." << endl;
        ifstream check_tok("network.tok");
        if (check_tok) {
            string line;
            int line_count = 0;
            while (getline(check_tok, line)) {
                if (line_count < 5) { // покажем первые 5 строк
                    cout << "network.tok строка " << line_count << ": " << line << endl;
                }
                line_count++;
            }
            check_tok.close();
            cout << "Всего строк в network.tok: " << line_count << endl;
        }
        else {
            cout << "Файл network.tok не создан!" << endl;
        }

        calculateLossDistribution(); // разделение потерь
        cout << "\nРезультаты записаны в файлы:\n"
            << "  network.out  – исходные данные и итерации\n"
            << "  network.rez  – результаты по узлам и ветвям\n"
            << "  network.rip  – потери по районам\n"
            << "  network.tok  – токи в ветвях\n"
            << "  losses.rez   – разделение потерь\n";
    }

    fout.close();

    cout << "\nНажмите Enter для завершения...";
    cin.ignore(); // очищаем буфер от предыдущего ввода
    cin.get();

    return 0;
}

// Чтение и подготовка данных
bool preparation(const string& fname)
{
    ifstream net(fname);
    if (!net) {
        cerr << "Не могу открыть файл " << fname << "\n";
        return false;
    }

    // Инициализация массивов районов
    for (int i = 0; i < 10; i++) nus[i] = 0;

    int j = 0, k = 0;
    int NUL[6];
    float AA[10];

    while (net >> NUL[1])
    {
        switch (NUL[1])
        {
        case 102: // slack
            net >> NUL[2] >> nn[0] >> unom[0] >> p0[0] >> q0[0]
                >> AA[1] >> AA[2] >> AA[3] >> AA[4] >> AA[5];
            nk[0] = 3;
            g[0] = b[0] = 0;
            raion(nn[0]);
            break;

        case 201: // PV-PQ
            ++j;
            net >> NUL[2] >> nn[j] >> unom[j] >> p0[j] >> q0[j]
                >> AA[1] >> AA[2] >> AA[3] >> AA[4] >> AA[5];
            nk[j] = 1;
            g[j] = b[j] = 0;
            if (AA[3] > 0.1) {
                unom[j] = AA[3];
                nk[j] = 2;
            }
            raion(nn[j]);
            break;

        case 301: // ветвь
            ++k;
            net >> NUL[2] >> nm[1][k] >> nm[2][k];
            if (nm[2][k] == 0)
            { // шунт
                net >> g[k] >> b[k];
                for (int l = 1; l <= j; ++l)
                    if (nn[l] == nm[1][k]) {
                        b[l] = -b[k];
                        b[k] = 0;
                    }
                --k;
            }
            else
            {
                net >> r[k] >> x[k] >> by[k] >> kt[k] >> AA[3] >> AA[4] >> AA[5];
                if (fabs(x[k]) < 1.001f) x[k] = 1.01f;
                if (kt[k] < 0.001f) kt[k] = 1.0f;
                by[k] = -by[k];
                gy[k] = 0;
            }
            break;
        default:
            net.ignore(1000, '\n');
            break;
        }
    }
    n = j;
    m = k;

    // Проверки связности
    ofstream ferr("network.err");
    bool ok = true;

    if (n == 0 || m == 0) ok = false;

    for (int i = 1; i <= n; ++i)
    {
        bool linked = false;
        for (int br = 1; br <= m; ++br)
            if (nn[i] == nm[1][br] || nn[i] == nm[2][br]) linked = true;
        if (!linked) {
            ok = false;
            ferr << "Нет ветвей для узла " << nn[i] << "\n";
        }
    }

    for (int br = 1; br <= m; ++br)
    {
        bool n1 = false, n2 = false;
        for (int i = 0; i <= n; ++i)
        {
            if (nm[1][br] == nn[i]) n1 = true;
            if (nm[2][br] == nn[i]) n2 = true;
        }
        if (!(n1 && n2)) {
            ok = false;
            ferr << "Ошибка связи ветви " << nm[1][br] << "-" << nm[2][br] << "\n";
        }
    }
    ferr.close();

    // Масштабирование
    for (int i = 0; i <= n; ++i) {
        g[i] *= 1e-6f;
        b[i] *= 1e-6f;
    }

    for (int i = 1; i <= m; ++i) {
        gy[i] *= 1e-6f;
        by[i] *= 1e-6f;
    }

    // Формируем nm1 – внутренние номера
    for (int i = 1; i <= m; ++i)
    {
        if (nm[1][i] == nn[0]) nm1[1][i] = 0;
        if (nm[2][i] == nn[0]) nm1[2][i] = 0;

        for (int j = 1; j <= n; ++j)
        {
            if (nm[1][i] == nn[j]) nm1[1][i] = j;
            if (nm[2][i] == nn[j]) nm1[2][i] = j;
        }
    }

    return ok;
}

// Задание начальных приближений
void zerovalue()
{
    for (int i = 0; i <= n; ++i)
    {
        va[i] = unom[i];
        vr[i] = 0;
    }
}

// Формирование матрицы проводимостей
void ylfl()
{
    for (int i = 1; i <= m; ++i)
    {
        float c = r[i] * r[i] + x[i] * x[i];
        gr[i] = r[i] / c;
        bx[i] = -x[i] / c;
    }

    for (int i = 0; i <= n; ++i)
    {
        gg[i] = g[i];
        bb[i] = b[i];
    }

    for (int j = 1; j <= m; ++j)
    {
        int i1 = nm1[1][j], i2 = nm1[2][j];
        gg[i1] += gr[j] / (kt[j] * kt[j]) + gy[j] / 2.f;
        bb[i1] += bx[j] / (kt[j] * kt[j]) + by[j] / 2.f;
        gg[i2] += gr[j] + gy[j] / 2.f;
        bb[i2] += bx[j] + by[j] / 2.f;
    }
}

// Расчет невязки
float func(int count)
{
    int i, i1, i2;
    float w = 0, h, pp;

    for (i = 0; i <= n; ++i)
    {
        ja[i] = gg[i] * va[i] - bb[i] * vr[i];
        jr[i] = bb[i] * va[i] + gg[i] * vr[i];
    }

    for (int j = 1; j <= m; ++j)
    {
        i1 = nm1[1][j];
        i2 = nm1[2][j];
        ja[i1] -= (gr[j] * va[i2] - bx[j] * vr[i2]) / kt[j];
        jr[i1] -= (bx[j] * va[i2] + gr[j] * vr[i2]) / kt[j];
        ja[i2] -= (gr[j] * va[i1] - bx[j] * vr[i1]) / kt[j];
        jr[i2] -= (bx[j] * va[i1] + gr[j] * vr[i1]) / kt[j];
    }

    for (i = 0; i <= n; ++i)
    {
        p[i] = va[i] * ja[i] + vr[i] * jr[i];
        q[i] = vr[i] * ja[i] - va[i] * jr[i];

        if (i > 0)
        {
            ds[2 * i] = -(p[i] + p0[i]);
            if (nk[i] == 1)
            {
                ds[2 * i - 1] = -(q[i] + q0[i]);
                h = fabs(q0[i]) < 1 ? ds[2 * i - 1] : ds[2 * i - 1] / q0[i];
            }
            else
            {
                ds[2 * i - 1] = -(va[i] * va[i] + vr[i] * vr[i] - unom[i] * unom[i]) / unom[i];
                h = ds[2 * i - 1] / unom[i];
            }
            pp = fabs(p0[i]) < 1 ? ds[2 * i] : ds[2 * i] / p0[i];
            w += pp * pp + h * h;
        }
    }

    return sqrtf(w / (2 * n));
}

// Формирование матрицы Якоби
void jacoby()
{
    for (int i = 1; i <= 2 * n; ++i)
        for (int j = 1; j <= 2 * n; ++j)
            A[i][j] = 0;

    for (int i = 1; i <= n; ++i)
    {
        if (nk[i] == 1)
        {
            A[2 * i - 1][2 * i - 1] = -bb[i] * va[i] + gg[i] * vr[i] - jr[i];
            A[2 * i - 1][2 * i] = -gg[i] * va[i] - bb[i] * vr[i] + ja[i];
        }
        else
        {
            A[2 * i - 1][2 * i - 1] = 2 * va[i] / unom[i];
            A[2 * i - 1][2 * i] = 2 * vr[i] / unom[i];
        }
        A[2 * i][2 * i - 1] = gg[i] * va[i] + bb[i] * vr[i] + ja[i];
        A[2 * i][2 * i] = -bb[i] * va[i] + gg[i] * vr[i] + jr[i];
    }

    for (int j = 1; j <= m; ++j)
    {
        int i1 = nm1[1][j], i2 = nm1[2][j];
        int j1 = 2 * i1 - 1, j2 = 2 * i1, j3 = 2 * i2 - 1, j4 = 2 * i2;

        if (i1 != 0 && i2 != 0)
        {
            if (nk[i1] == 1)
            {
                A[j1][j3] = -(-bx[j] * va[i1] + gr[j] * vr[i1]) / kt[j];
                A[j1][j4] = -(-gr[j] * va[i1] - bx[j] * vr[i1]) / kt[j];
            }
            A[j2][j3] = -(gr[j] * va[i1] + bx[j] * vr[i1]) / kt[j];
            A[j2][j4] = -(-bx[j] * va[i1] + gr[j] * vr[i1]) / kt[j];

            if (nk[i2] == 1)
            {
                A[j3][j1] = -(-bx[j] * va[i2] + gr[j] * vr[i2]) / kt[j];
                A[j3][j2] = -(-gr[j] * va[i2] - bx[j] * vr[i2]) / kt[j];
            }
            A[j4][j1] = -(gr[j] * va[i2] + bx[j] * vr[i2]) / kt[j];
            A[j4][j2] = -(-bx[j] * va[i2] + gr[j] * vr[i2]) / kt[j];
        }
    }
}

// Решение системы методом Гаусса
void gauss()
{
    float u[2 * inn];

    for (int i = 1; i <= 2 * n - 1; ++i)
    {
        float c = A[i][i];
        if (fabs(c) < 1e-7f) c = 1e-7f;

        for (int j = i + 1; j <= 2 * n; ++j)
            if (fabs(A[i][j]) > 1e-7f)
                A[i][j] /= c;

        ds[i] /= c;

        for (int k = i + 1; k <= 2 * n; ++k)
        {
            float d = A[k][i];
            if (fabs(d) > 1e-7f)
            {
                for (int l = i + 1; l <= 2 * n; ++l)
                    if (fabs(A[i][l]) > 1e-7f)
                        A[k][l] -= A[i][l] * d;
                ds[k] -= ds[i] * d;
            }
        }
    }

    u[2 * n] = ds[2 * n] / A[2 * n][2 * n];

    for (int k = 2 * n - 1; k >= 1; --k)
    {
        float s = 0;
        for (int j = k + 1; j <= 2 * n; ++j)
            if (fabs(A[k][j]) > 1e-7f)
                s += A[k][j] * u[j];
        u[k] = ds[k] - s;
    }

    for (int i = 1; i <= n; ++i)
    {
        va[i] += u[2 * i - 1];
        vr[i] += u[2 * i];
    }
}

// Функция формирования выходных файлов
bool load()
{
    ofstream net2("network.rez");
    ofstream tok("network.tok");
    ofstream raipot("network.rip");

    if (!net2 || !tok || !raipot) {
        cerr << "Невозможно создать один из выходных файлов\n";
        return false;
    }

    net2.setf(ios::fixed | ios::showpoint);
    raipot.setf(ios::fixed | ios::showpoint);
    tok.setf(ios::fixed | ios::showpoint);

    // === ФАЙЛ NETWORK.REZ ===
    net2 << "          Результаты расчета по узлам\n";
    net2 << "    N        V         dV         P         Q        Pg       Qb\n";

    float sp = 0, sq = 0, spg = 0, sqb = 0, pg, qb, mv, dv, i1a, i1r, i2a, i2r, p12, q12, p21, q21, dPsum = 0;

    // Переменные для районов
    float saldo[2][10] = { 0 };
    float sumpot[10] = { 0 };
    int sv[10] = { 0 };
    float line[10][10] = { 0 };
    int linc[10][10] = { 0 };
    float tran[10][13] = { 0 };
    int trn[10] = { 0 };
    int nuzlN[10][13] = { 0 }, nuzlK[10][13] = { 0 };

    // === ФАЙЛ NETWORK.TOK ===
    // ЗАПИСЫВАЕМ В ФОРМАТЕ ДЛЯ ЧТЕНИЯ В calculateLossDistribution()
    tok << "   N1   N2      Iа       Iр     R\n";

    // Результаты по узлам
    for (int i = 0; i <= n; i++) {
        mv = va[i] * va[i] + vr[i] * vr[i];
        dv = atan2(vr[i], va[i]) * 57.295779515f;
        pg = mv * g[i];
        qb = -mv * b[i];
        sp += p[i];
        sq += q[i];
        spg += pg;
        sqb += qb;
        mv = sqrtf(mv);

        net2 << setw(5) << nn[i]
            << setw(10) << setprecision(3) << mv
            << setw(10) << setprecision(3) << dv
            << setw(10) << setprecision(3) << -p[i]
            << setw(10) << setprecision(3) << -q[i]
            << setw(10) << setprecision(3) << pg
            << setw(10) << setprecision(3) << qb
            << endl;
    }

    net2 << "---------------------------------------------------\n";
    net2 << "Баланс пассивных элементов ";
    net2 << setw(10) << setprecision(3) << sp
        << setw(10) << setprecision(3) << sq
        << setw(10) << setprecision(3) << spg
        << setw(10) << setprecision(3) << sqb
        << endl;
    net2 << "                         + потребление, - генерация";
    net2 << " \n";
    net2 << " \n";
    net2 << "                   Результаты расчета по ветвям\n";
    net2 << "   N1   N2       P12       Q12       P21       Q21       dP\n";

    for (int j = 1; j <= m; j++) {
        int i1 = nm1[1][j], i2 = nm1[2][j];

        // ВАЖНО: правильный расчет токов
        i1a = (va[i1] * gr[j] - vr[i1] * bx[j]) / kt[j] / kt[j] - (va[i2] * gr[j] - vr[i2] * bx[j]) / kt[j];
        i1r = (va[i1] * bx[j] + vr[i1] * gr[j]) / kt[j] / kt[j] - (va[i2] * bx[j] + vr[i2] * gr[j]) / kt[j];
        i2a = (va[i1] * gr[j] - vr[i1] * bx[j]) / kt[j] - (va[i2] * gr[j] - vr[i2] * bx[j]);
        i2r = (va[i1] * bx[j] + vr[i1] * gr[j]) / kt[j] - (va[i2] * bx[j] + vr[i2] * gr[j]);

        mv = va[i1] * va[i1] + vr[i1] * vr[i1];
        p12 = va[i1] * i1a + vr[i1] * i1r - gy[j] * mv / 2.f;
        q12 = vr[i1] * i1a - va[i1] * i1r - by[j] * mv / 2.f;

        mv = va[i2] * va[i2] + vr[i2] * vr[i2];
        p21 = va[i2] * i2a + vr[i2] * i2r + gy[j] * mv / 2.f;
        q21 = vr[i2] * i2a - va[i2] * i2r + by[j] * mv / 2.f;

        float dP = fabs(p21 - p12);
        dPsum += dP;

        // ЗАПИСЬ В ФАЙЛ ТОКОВ - ФАКТИЧЕСКИЕ НОМЕРА УЗЛОВ И ТОК НАЧАЛА ВЕТВИ
        tok << setw(5) << nn[i1] << setw(5) << nn[i2]
            << setw(10) << setprecision(4) << i1a  // ток активный начала
            << setw(10) << setprecision(4) << i1r  // ток реактивный начала  
            << setw(10) << setprecision(3) << r[j] << endl;

        // Запись в файл результатов по ветвям
        net2 << setw(5) << nn[i1] << setw(5) << nn[i2]
            << setw(10) << setprecision(3) << -p12
            << setw(10) << setprecision(3) << -q12
            << setw(10) << setprecision(3) << p21
            << setw(10) << setprecision(3) << q21
            << setw(10) << setprecision(3) << dP
            << endl;

        // Расчет по районам
        int k = nn[i1] / kkk;
        while (k > 9) k /= kk;
        int krat = nn[i2] / kkk;
        while (krat > 9) krat /= kk;

        if (krat == k) {
            sv[k]++;
            sumpot[k] += dP;

            if (((kt[j] - 1) < 0.001) && ((kt[j] - 1) > -0.001)) {
                if (unom[i1] <= 8) {
                    line[k][0] += dP; linc[k][0]++;
                }
                else if (unom[i1] > 8 && unom[i1] <= 15) {
                    line[k][1] += dP; linc[k][1]++;
                }
                else if (unom[i1] > 15 && unom[i1] <= 28) {
                    line[k][2] += dP; linc[k][2]++;
                }
                else if (unom[i1] > 28 && unom[i1] <= 70) {
                    line[k][3] += dP; linc[k][3]++;
                }
                else if (unom[i1] > 70 && unom[i1] <= 140) {
                    line[k][4] += dP; linc[k][4]++;
                }
                else if (unom[i1] > 140 && unom[i1] <= 270) {
                    line[k][5] += dP; linc[k][5]++;
                }
                else if (unom[i1] > 270 && unom[i1] <= 430) {
                    line[k][6] += dP; linc[k][6]++;
                }
                else if (unom[i1] > 430 && unom[i1] <= 600) {
                    line[k][7] += dP; linc[k][7]++;
                }
                else if (unom[i1] > 600 && unom[i1] <= 900) {
                    line[k][8] += dP; linc[k][8]++;
                }
                else if (unom[i1] > 900) {
                    line[k][9] += dP; linc[k][9]++;
                }
            }
            else {
                if (trn[k] < 13) {
                    tran[k][trn[k]] += dP;
                    nuzlN[k][trn[k]] = nn[i1];
                    nuzlK[k][trn[k]] = nn[i2];
                    trn[k]++;
                }
            }
        }
        else {
            saldo[0][k] += -p12;
            saldo[1][k] += -q12;
            saldo[0][krat] += p21;
            saldo[1][krat] += q21;
        }
    }

    net2 << setw(60) << setprecision(3) << dPsum << endl;
    net2.close();
    tok.close();

    // === ФАЙЛ NETWORK.RIP ===
    int ul[10] = { 6, 10, 20, 35, 110, 220, 330, 500, 750, 1150 };

    for (int i = 0; i < 10; i++) {
        if (sv[i] == 0) continue;
        raipot << "Район  № " << setw(5) << i << endl;

        for (int g = 0; g < 10; g++) {
            if (linc[i][g] != 0) {
                raipot << " Потери в линиях" << endl;
                raipot << setw(5) << ul[g] << " кВ" << setw(15) << setprecision(3) << line[i][g] << endl;
            }
        }

        if (trn[i] > 0) {
            float SumTrPot = 0;
            for (int f = 0; f < trn[i]; f++) {
                SumTrPot += tran[i][f];
            }
            raipot << " Потери в трансформаторах текущего района  "
                << setw(10) << setprecision(3) << SumTrPot << endl;
        }
    }

    raipot << endl << "№ района          Сальдо P           Сальдо Q       Сумм. потери в районе" << endl;

    float sumoll = 0;
    for (int k = 0; k < 10; k++) {
        if (nus[k] != 0) {
            raipot << setw(5) << k
                << setw(20) << setprecision(3) << saldo[0][k]
                << setw(20) << setprecision(3) << saldo[1][k]
                << setw(20) << setprecision(3) << sumpot[k] << endl;
            sumoll += sumpot[k];
        }
    }

    raipot << " Суммарные потери в сетях районов " << setw(25) << setprecision(3) << sumoll << endl;
    raipot.close();

    return true;
}