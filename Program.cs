using System.Globalization;
using System.Text;

class Program
{
    const int MAX_CUENTAS = 200;
    const int MAX_MOVIMIENTOS = 5000;
    const string RUTA_CUENTAS = "Datos/cuentas.csv";
    const string RUTA_MOVIMIENTOS = "Datos/movimientos.csv";

    static string[,] cuentas = new string[MAX_CUENTAS, 4];
    static int totalCuentas = 0;

    static string[,] movTexto = new string[MAX_MOVIMIENTOS, 3];
    static decimal[,] movMontos = new decimal[MAX_MOVIMIENTOS, 2];
    static int totalMovimientos = 0;

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Directory.CreateDirectory("Datos");
        CargarCuentas();
        CargarMovimientos();

        int opcion = 0;
        while (opcion != 9)
        {
            MostrarMenu();
            int.TryParse(Console.ReadLine(), out opcion);

            switch (opcion)
            {
                case 1: AgregarCuenta(); break;
                case 2: RegistrarMovimiento(); break;
                case 3: ConsultarCuentas(); break;
                case 4: ConsultarMovimientos(); break;
                case 5: EstadoResultados(); break;
                case 6: EstadoSituacionFinanciera(); break;
                case 7: EstadoCambiosPatrimonio(); break;
                case 8: EstadoFlujosEfectivo(); break;
                case 9: Console.WriteLine("Hasta luego."); break;
                default: Console.WriteLine("Opción inválida."); break;
            }
        }
    }

    static void MostrarMenu()
    {
        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("       SISTEMA CONTABLE");
        Console.WriteLine("========================================");
        Console.WriteLine();
        Console.WriteLine("1. Administrar catálogo de cuentas");
        Console.WriteLine("2. Registrar movimiento contable");
        Console.WriteLine("3. Consultar catálogo de cuentas");
        Console.WriteLine("4. Consultar movimientos");
        Console.WriteLine("5. Estado de Resultados");
        Console.WriteLine("6. Estado de Situación Financiera");
        Console.WriteLine("7. Estado de Cambios en el Patrimonio");
        Console.WriteLine("8. Estado de Flujos de Efectivo");
        Console.WriteLine("9. Salir");
        Console.WriteLine();
        Console.Write("Seleccione una opción: ");
    }

    static void CargarCuentas()
    {
        totalCuentas = 0;
        if (!File.Exists(RUTA_CUENTAS)) return;

        try
        {
            string[] lineas = File.ReadAllLines(RUTA_CUENTAS);
            for (int i = 1; i < lineas.Length; i++)
            {
                string[] campos = lineas[i].Split(';');
                if (campos.Length < 4) continue;

                for (int c = 0; c < 4; c++)
                    cuentas[totalCuentas, c] = campos[c].Trim();
                totalCuentas++;
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine("Error al leer cuentas: " + ex.Message);
        }
    }

    static void GuardarCuentas()
    {
        try
        {
            List<string> lineas = new List<string>();
            lineas.Add("Codigo;Nombre;Tipo;Naturaleza");
            for (int i = 0; i < totalCuentas; i++)
                lineas.Add(cuentas[i, 0] + ";" + cuentas[i, 1] + ";" + cuentas[i, 2] + ";" + cuentas[i, 3]);
            File.WriteAllLines(RUTA_CUENTAS, lineas);
        }
        catch (IOException ex)
        {
            Console.WriteLine("Error al guardar cuentas: " + ex.Message);
        }
    }

    static void CargarMovimientos()
    {
        totalMovimientos = 0;
        if (!File.Exists(RUTA_MOVIMIENTOS)) return;

        try
        {
            string[] lineas = File.ReadAllLines(RUTA_MOVIMIENTOS);
            for (int i = 1; i < lineas.Length; i++)
            {
                string[] campos = lineas[i].Split(';');
                if (campos.Length < 5) continue;

                movTexto[totalMovimientos, 0] = campos[0];
                movTexto[totalMovimientos, 1] = campos[1];
                movTexto[totalMovimientos, 2] = campos[4];
                movMontos[totalMovimientos, 0] = decimal.Parse(campos[2], CultureInfo.InvariantCulture);
                movMontos[totalMovimientos, 1] = decimal.Parse(campos[3], CultureInfo.InvariantCulture);
                totalMovimientos++;
            }
        }
        catch (Exception ex) when (ex is IOException || ex is FormatException)
        {
            Console.WriteLine("Error al leer movimientos: " + ex.Message);
        }
    }

    static void GuardarMovimientos()
    {
        try
        {
            List<string> lineas = new List<string>();
            lineas.Add("Fecha;Cuenta;Debe;Haber;Concepto");
            for (int i = 0; i < totalMovimientos; i++)
            {
                lineas.Add(movTexto[i, 0] + ";" + movTexto[i, 1] + ";" +
                           movMontos[i, 0].ToString("F2", CultureInfo.InvariantCulture) + ";" +
                           movMontos[i, 1].ToString("F2", CultureInfo.InvariantCulture) + ";" +
                           movTexto[i, 2]);
            }
            File.WriteAllLines(RUTA_MOVIMIENTOS, lineas);
        }
        catch (IOException ex)
        {
            Console.WriteLine("Error al guardar movimientos: " + ex.Message);
        }
    }

    static int BuscarCuenta(string codigo)
    {
        for (int i = 0; i < totalCuentas; i++)
            if (cuentas[i, 0] == codigo) return i;
        return -1;
    }

    static void AgregarCuenta()
    {
        Console.Write("Código: ");
        string codigo = (Console.ReadLine() ?? "").Trim();
        if (codigo == "" || BuscarCuenta(codigo) != -1)
        {
            Console.WriteLine("Código vacío o ya existe.");
            return;
        }

        Console.Write("Nombre: ");
        string nombre = (Console.ReadLine() ?? "").Trim();

        Console.Write("Tipo (Activo/Pasivo/Patrimonio/Ingreso/Gasto): ");
        string tipo = (Console.ReadLine() ?? "").Trim();

        Console.Write("Naturaleza (Deudora/Acreedora): ");
        string naturaleza = (Console.ReadLine() ?? "").Trim();

        cuentas[totalCuentas, 0] = codigo;
        cuentas[totalCuentas, 1] = nombre;
        cuentas[totalCuentas, 2] = tipo;
        cuentas[totalCuentas, 3] = naturaleza;
        totalCuentas++;

        GuardarCuentas();
        Console.WriteLine("Cuenta agregada.");
    }

    static void ConsultarCuentas()
    {
        Console.WriteLine();
        Console.WriteLine($"{"Código",-10}{"Nombre",-32}{"Tipo",-13}{"Naturaleza",-10}");
        for (int i = 0; i < totalCuentas; i++)
            Console.WriteLine($"{cuentas[i, 0],-10}{cuentas[i, 1],-32}{cuentas[i, 2],-13}{cuentas[i, 3],-10}");
    }

    static void RegistrarMovimiento()
    {
        Console.Write("Fecha (dd/MM/yyyy): ");
        string fecha = (Console.ReadLine() ?? "").Trim();

        Console.Write("Concepto: ");
        string concepto = (Console.ReadLine() ?? "").Trim();

        int inicio = totalMovimientos;
        decimal totalDebe = 0;
        decimal totalHaber = 0;

        while (true)
        {
            Console.Write("Código de cuenta (ENTER para terminar): ");
            string codigo = (Console.ReadLine() ?? "").Trim();
            if (codigo == "") break;

            if (BuscarCuenta(codigo) == -1)
            {
                Console.WriteLine("La cuenta no existe.");
                continue;
            }

            Console.Write("Debe: ");
            decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal debe);
            Console.Write("Haber: ");
            decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal haber);

            movTexto[totalMovimientos, 0] = fecha;
            movTexto[totalMovimientos, 1] = codigo;
            movTexto[totalMovimientos, 2] = concepto;
            movMontos[totalMovimientos, 0] = debe;
            movMontos[totalMovimientos, 1] = haber;
            totalMovimientos++;

            totalDebe += debe;
            totalHaber += haber;
        }

        if (totalDebe != totalHaber || totalDebe == 0)
        {
            Console.WriteLine("El asiento no cuadra. No se guardó.");
            totalMovimientos = inicio;
            return;
        }

        GuardarMovimientos();
        Console.WriteLine("Movimiento registrado.");
    }

    static void ConsultarMovimientos()
    {
        Console.WriteLine();
        Console.WriteLine($"{"Fecha",-12}{"Cuenta",-10}{"Debe",14}{"Haber",14}  Concepto");
        for (int i = 0; i < totalMovimientos; i++)
        {
            Console.WriteLine($"{movTexto[i, 0],-12}{movTexto[i, 1],-10}" +
                              $"{movMontos[i, 0],14:N2}{movMontos[i, 1],14:N2}  {movTexto[i, 2]}");
        }
    }

    static void EstadoResultados()
    {
        Console.WriteLine("Pendiente.");
    }

    static void EstadoSituacionFinanciera()
    {
        Console.WriteLine("Pendiente.");
    }

    static void EstadoCambiosPatrimonio()
    {
        Console.WriteLine("Pendiente.");
    }

    static void EstadoFlujosEfectivo()
    {
        Console.WriteLine("Pendiente.");
    }
}