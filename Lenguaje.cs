/*
    REQUERIMIENTOS
    1. Excepción en el Console.Read()
    2. La segunda asignación del for (incremento) debe de ejecutarse después del bloque
       de instrucciones o instrucción
    3. Programar FuncionMatematica Metodo (MatFunction)
    4. Programar el For 
    5. Programar el While

*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Emulador
{
    public class Lenguaje : Sintaxis
    {
        Stack<float> s;
        List<Variable> l;
        Variable.TipoDato maximoTipo;
        public Lenguaje() : base()
        {
            s = new Stack<float>();
            l = new List<Variable>();
            maximoTipo = Variable.TipoDato.Char;
            log.WriteLine("Constructor lenguaje");
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            s = new Stack<float>();
            l = new List<Variable>();
            maximoTipo = Variable.TipoDato.Char;
            log.WriteLine("Constructor lenguaje");
        }

        private void displayStack()
        {
            Console.WriteLine("Contenido del stack: ");
            foreach (float elemento in s)
            {
                Console.WriteLine(elemento);
            }
        }

        private void displayLista()
        {
            log.WriteLine("Lista de variables: ");
            foreach (Variable elemento in l)
            {
                log.WriteLine($"{elemento.getNombre()} {elemento.GetTipoDato()} {elemento.getValor()}");
            }
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (Contenido == "using")
            {
                Librerias();
            }
            if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
            Main();
            displayLista();
        }
        //Librerias -> using ListaLibrerias; Librerias?

        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(";");
            if (Contenido == "using")
            {
                Librerias();
            }
        }
        //Variables -> tipo_dato Lista_identificadores; Variables?

        private void Variables()
        {
            Variable.TipoDato t = Variable.TipoDato.Char;
            switch (Contenido)
            {
                case "int": t = Variable.TipoDato.Int; break;
                case "float": t = Variable.TipoDato.Float; break;
            }
            match(Tipos.TipoDato);
            ListaIdentificadores(t);
            match(";");
            if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
        }
        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (Contenido == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }
        //ListaIdentificadores -> identificador (= Expresion)? (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoDato t)
        {
            if (l.Find(variable => variable.getNombre() == Contenido) != null)
            {
                throw new Error($"La variable {Contenido} ya existe", log, linea, columna);
            }
            Variable v = new Variable(t, Contenido);
            l.Add(v);
            match(Tipos.Identificador);
            if (Contenido == "=")
            {
                match("=");
                if (Contenido == "Console")
                {
                    match("Console");
                    match(".");
                    if (Contenido == "Read")
{
    match("Read");
    try
    {
        int r = Console.Read();
        if (maximoTipo > Variable.valorToTipoDato(r))
        {
            throw new Error("Tipo Dato. No esta permitido asignar un valor " + maximoTipo + " a una variable " + Variable.valorToTipoDato(r), log, linea, columna);
        }
        v.setValor(r);
    }
    catch (System.IO.IOException ex)
    {
        throw new Error("Error de E/S al leer desde la consola: " + ex.Message, log, linea, columna);
    }
    catch (OutOfMemoryException ex)
    {
        throw new Error("Error de memoria al leer desde la consola: " + ex.Message, log, linea, columna);
    }
    catch (Exception ex)
    {
        throw new Error("Error inesperado en Console.Read(): " + ex.Message, log, linea, columna);
    }
}
                    else
                    {
                        match("ReadLine");
                        string? r = Console.ReadLine();
                        if (float.TryParse(r, out float valor))
                        {
                            if (maximoTipo > Variable.valorToTipoDato(valor))
                            {
                                throw new Error("Tipo Dato. No esta permitido asignar un valor " + maximoTipo + "a una variable " + Variable.valorToTipoDato(valor), log, linea, columna);
                            }
                            v.setValor(valor);
                        }
                        else
                        {
                            throw new Error("Sintaxis. No se ingresó un número ", log, linea, columna);
                        }
                    }
                    match("(");
                    match(")");
                }
                else
                {
                    // Como no se ingresó un número desde el Console, entonces viene de una expresión matemática
                    Expresion();
                    float resultado = s.Pop();
                    l.Last().setValor(resultado);
                }
            }
            if (Contenido == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones(bool ejecuta)
        {
            match("{");
            if (Contenido != "}")
            {
                ListaInstrucciones(ejecuta);
            }
            else
            {
                match("}");
            }
        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool ejecuta)
        {
            Instruccion(ejecuta);
            if (Contenido != "}")
            {
                ListaInstrucciones(ejecuta);
            }
            else
            {
                match("}");
            }
        }

        //Instruccion -> console | If | While | do | For | Variables | Asignación
        private void Instruccion(bool ejecuta)
        {
            if (Contenido == "Console")
            {
                console(ejecuta);
            }
            else if (Contenido == "if")
            {
                If(ejecuta);
            }
            else if (Contenido == "while")
            {
                While(ejecuta);
            }
            else if (Contenido == "do")
            {
                Do(ejecuta);
            }
            else if (Contenido == "for")
            {
                For(ejecuta);
            }
            else if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
                match(";");
            }
        }
        //Asignacion -> Identificador = Expresion; (DONE)
        /*
        Id++ (DONE)
        Id-- (DONE)
        Id IncrementoTermino Expresion (DONE)
        Id IncrementoFactor Expresion (DONE)
        Id = Console.Read() (DONE)
        Id = Console.ReadLine() (DONE)
        */
        private void Asignacion()
        {
            // Cada vez que haya una asignación, reiniciar el maximoTipo
            maximoTipo = Variable.TipoDato.Char;
            float r;
            Variable? v = l.Find(variable => variable.getNombre() == Contenido);
            if (v == null)
            {
                throw new Error("Sintaxis: La variable " + Contenido + " no está definida", log, linea, columna);
            }
            match(Tipos.Identificador);

            if (Contenido == "++")
            {
                match("++");
                r = v.getValor() + 1;
                v.setValor(r, maximoTipo);
            }
            else if (Contenido == "--")
            {
                match("--");
                r = v.getValor() - 1;
                v.setValor(r, maximoTipo);
            }
            else if (Contenido == "=")
            {
                match("=");

                if (Contenido == "Console")
                {
                    ListaIdentificadores(v.GetTipoDato());
                }
                else
                {
                    Expresion();
                    r = s.Pop();
                    v.setValor(r, maximoTipo);
                }
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                r = v.getValor() + s.Pop();
                v.setValor(r, maximoTipo);
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                r = v.getValor() - s.Pop();
                v.setValor(r, maximoTipo);
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                r = v.getValor() * s.Pop();
                v.setValor(r, maximoTipo);
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                r = v.getValor() / s.Pop();
                v.setValor(r, maximoTipo);
            }
            else if (Contenido == "%=")
            {
                match("%=");
                Expresion();
                r = v.getValor() % s.Pop();
                v.setValor(r, maximoTipo);
            }
            else
            {
                match("ReadLine");
                string? read = Console.ReadLine();
                float result;
                if (float.TryParse(read, out result))
                {
                    v.setValor(result);
                }
                else
                {
                    throw new Error("Sintaxis: sólo se pueden ingresar números", log, linea, columna);
                }
            }
        }
        /*If -> if (Condicion) bloqueInstrucciones | instruccion
        (else bloqueInstrucciones | instruccion)?*/
        private void If(bool ejecuta2)
        {
            match("if");
            match("(");
            bool ejecuta = Condicion() && ejecuta2;
            //Console.WriteLine(ejecuta);
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }
            if (Contenido == "else")
            {
                match("else");
                bool ejecutarElse = ejecuta2 && !ejecuta; // Solo se ejecuta el else si el if no se ejecutó
                if (Contenido == "{")
                {
                    BloqueInstrucciones(ejecutarElse);
                }
                else
                {
                    Instruccion(ejecutarElse);
                }
            }
        }
        //Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion()
        {
            maximoTipo = Variable.TipoDato.Char;
            Expresion();
            float valor1 = s.Pop();
            string operador = Contenido;
            match(Tipos.OperadorRelacional);
            maximoTipo = Variable.TipoDato.Char;
            Expresion();
            float valor2 = s.Pop();
            switch (operador)
            {
                case ">": return valor1 > valor2;
                case ">=": return valor1 >= valor2;
                case "<": return valor1 < valor2;
                case "<=": return valor1 <= valor2;
                case "==": return valor1 == valor2;
                default: return valor1 != valor2;
            }
        }
        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool ejecuta)
        {
            match("while");
            match("(");
            bool condicionWhile = Condicion() && ejecuta;
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(true);
            }
            else
            {
                Instruccion(true);
            }
        }
        /*Do -> do bloqueInstrucciones | intruccion 
        while(Condicion);*/
        private void Do(bool ejecuta)
        {
            bool executeDo;
            int charTmp = Caracter - 3;
            int linetmp = Error.linea;
            do
            {
                match("do");

                if (Contenido == "{")
                {
                    BloqueInstrucciones(ejecuta);
                }
                else
                {
                    Instruccion(ejecuta);
                }
                match("while");
                match("(");
                executeDo = Condicion() && ejecuta;
                match(")");
                match(";");

                if (executeDo)
                {
                    archivo.BaseStream.Seek(charTmp, System.IO.SeekOrigin.Begin); // Corrección de 'seekOrigin' a 'SeekOrigin'
                    Caracter = charTmp; // Suponiendo que 'CaracterActual' es la variable correcta
                    nextToken(); // Asegúrate de que este método exista
                }
            }
            while (executeDo);
        }
        /*For -> for(Asignacion; Condicion; Asignacion) 
        BloqueInstrucciones | Intruccion*/
        private void For(bool ejecuta)
        {
            match("for");
            match("(");
            Asignacion();
            match(";");

            int posCond = (int)archivo.BaseStream.Position;
            int lineaCond = linea;

            bool executeFor = Condicion() && ejecuta;
            match(";");

            int posIncr = (int)archivo.BaseStream.Position;
            int lineaIncr = linea;

            Asignacion();
            match(")");

            int posCuerpo = (int)archivo.BaseStream.Position;
            int lineaCuerpo = linea;

            if (executeFor)
            {
                do
                {
                    // Ejecutar el cuerpo
                    archivo.BaseStream.Seek(posCuerpo, System.IO.SeekOrigin.Begin);
                    linea = lineaCuerpo;
                    nextToken();

                    if (Contenido == "{")
                    {
                        BloqueInstrucciones(true);
                    }
                    else
                    {
                        Instruccion(true);
                    }

                    // Ejecutar el incremento (segunda asignación)
                    archivo.BaseStream.Seek(posIncr, System.IO.SeekOrigin.Begin);
                    linea = lineaIncr;
                    nextToken();
                    Asignacion();

                    // Volver a evaluar la condición
                    archivo.BaseStream.Seek(posCond, System.IO.SeekOrigin.Begin);
                    linea = lineaCond;
                    nextToken();
                    executeFor = Condicion() && ejecuta;

                } while (executeFor);
            }
            if (Contenido == "{")
            {
                BloqueInstrucciones(false);
            }
            else
            {
                Instruccion(false);

            }
        }
        //Console -> Console.(WriteLine|Write) (cadena? concatenaciones?);
        private void console(bool ejecuta)
        {
            bool isWriteLine = false;
            match("Console");
            match(".");
            if (Contenido == "WriteLine")
            {
                match("WriteLine");
                isWriteLine = true;
            }
            else
            {
                match("Write");
            }
            match("(");
            string concatenaciones = "";
            if (Clasificacion == Tipos.Cadena)
            {
                concatenaciones = Contenido.Trim('"');
                match(Tipos.Cadena);
            }
            if (Contenido == "+")
            {
                match("+");
                concatenaciones += Concatenaciones();  // Se acumula el resultado de las concatenaciones
            }
            match(")");
            match(";");
            if (ejecuta)
            {
                if (isWriteLine)
                {
                    Console.WriteLine(concatenaciones);
                }
                else
                {
                    Console.Write(concatenaciones);
                }
            }
        }
        // Concatenaciones -> Identificador|Cadena ( + concatenaciones )?
        private string Concatenaciones()
        {
            string resultado = "";
            if (Clasificacion == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.getNombre() == Contenido);
                if (v != null)
                {
                    resultado = v.getValor().ToString(); // Obtener el valor de la variable y convertirla
                }
                else
                {
                    throw new Error("La variable " + Contenido + " no está definida", log, linea, columna);
                }
                match(Tipos.Identificador);
            }
            else if (Clasificacion == Tipos.Cadena)
            {
                resultado = Contenido.Trim('"');
                match(Tipos.Cadena);
            }
            if (Contenido == "+")
            {
                match("+");
                resultado += Concatenaciones();  // Acumula el siguiente fragmento de concatenación
            }
            return resultado;
        }
        //Main -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            BloqueInstrucciones(true);
        }
        // Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (Clasificacion == Tipos.OperadorTermino)
            {
                string operador = Contenido;
                match(Tipos.OperadorTermino);
                Termino();
                //Console.Write(operador + " ");
                float n1 = s.Pop();
                float n2 = s.Pop();
                switch (operador)
                {
                    case "+": s.Push(n2 + n1); break;
                    case "-": s.Push(n2 - n1); break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (Clasificacion == Tipos.OperadorFactor)
            {
                string operador = Contenido;
                match(Tipos.OperadorFactor);
                Factor();
                //Console.Write(operador + " ");
                float n1 = s.Pop();
                float n2 = s.Pop();
                switch (operador)
                {
                    case "*": s.Push(n2 * n1); break;
                    case "/": s.Push(n2 / n1); break;
                    case "%": s.Push(n2 % n1); break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (Clasificacion == Tipos.Numero)
            {
                //Si el tipo de dato del número es mayor al tipo de dato actual, cambiarlo
                if (maximoTipo < Variable.valorToTipoDato(float.Parse(Contenido)))
                {
                    maximoTipo = Variable.valorToTipoDato(float.Parse(Contenido));
                }
                s.Push(float.Parse(Contenido));
                //Console.Write(Contenido + " ");
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.getNombre() == Contenido);
                if (v == null)
                {
                    throw new Error("Sintaxis: la variable " + Contenido + " no está definida", log, linea, columna);
                }
                if (maximoTipo < v.GetTipoDato())
                {
                    maximoTipo = v.GetTipoDato();
                }
                s.Push(v.getValor());
                //Console.Write(Contenido + " ");
                match(Tipos.Identificador);
            }
            else if (this.Clasificacion == Tipos.FuncionMatematica)
            {
                string funcionMath = Contenido;
                match(Tipos.FuncionMatematica);

                match("(");
                Expresion();
                match(")");

                float resultado = s.Pop();

                float resultadoMath = FuncionMatematica(resultado, funcionMath);
                s.Push(resultadoMath);
            }
            else
            {
                match("(");
                Variable.TipoDato tipoCasteo = Variable.TipoDato.Char;
                bool huboCasteo = false;
                if (Clasificacion == Tipos.TipoDato)
                {
                    switch (Contenido)
                    {
                        case "int": tipoCasteo = Variable.TipoDato.Int; break;
                        case "float": tipoCasteo = Variable.TipoDato.Float; break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                    huboCasteo = true;
                }
                Expresion();
                if (huboCasteo)
                {
                    maximoTipo = tipoCasteo;
                    float r = s.Pop();
                    switch (tipoCasteo)
                    {
                        case Variable.TipoDato.Int: r = (r % 65536); break;
                        case Variable.TipoDato.Char: r = (r % 256); break;
                    }
                    s.Push(r);
                }
                match(")");
            }
        }
        private float FuncionMatematica(float valor, string nombre)
        {
            float resultado = valor;

            switch (nombre)
            {
                case "abs": 
            resultado = Math.Abs(valor); 
            break;
        case "ceil": 
            resultado = (float)Math.Ceiling(valor); // Corrección de math.ceiling a Math.Ceiling
            break;
        case "pow": 
            resultado = (float)Math.Pow(valor, 2); // Añadimos el segundo parámetro que faltaba
            break;
        case "sqrt": 
            resultado = (float)Math.Sqrt(valor); // Corrección de math.sqrt a Math.Sqrt
            break;
        case "exp": 
            resultado = (float)Math.Exp(valor); 
            break;
        case "floor": 
            resultado = (float)Math.Floor(valor); 
            break;
        case "max":
            resultado = Math.Max(valor, 0); // Asumimos que queremos el máximo entre el valor y 0
            break;
        case "log10": 
            resultado = (float)Math.Log10(valor); // Corrección de math.log10 a Math.Log10
            break;
        case "log2": 
            resultado = (float)Math.Log(valor, 2); // No existe Math.Log2 directamente, usamos Log con base 2
            break;
        case "rand":
            Random random = new Random();
            resultado = (float)random.NextDouble() * valor; // Corrección del operador = a *
            break;
        case "trunc": 
            resultado = (float)Math.Truncate(valor); 
            break;
        case "round": 
            resultado = (float)Math.Round(valor); 
            break;

            }
            /*if(nombre == "abs"){
                return Math.Abs (valor);
            } else if (nombre == "pow"){
                return (float) Math.Pow (valor, 2);
            }*/

            return resultado;
        }
        /*SNT = Producciones = Invocar el metodo
        ST  = Tokens (Contenido | Classification) = Invocar match    Variables -> tipo_dato Lista_identificadores; Variables?*/
    }
}

