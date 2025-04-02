using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emulador
{
    public class Variable
    {
        public enum TipoDato
        {
            Char, Int, Float
        }
        TipoDato tipo;
        string nombre;
        float valor;

        public Variable(TipoDato tipo, string nombre, float valor = 0)
        {
            this.tipo = tipo;
            this.nombre = nombre;
            this.valor = valor;
        }

        public void setValor(float valor)
        {
            TipoDato tipoValor = valorToTipoDato(valor);

            if (tipoValor > tipo)
            {
                throw new Error($"Semántico: No se puede asignar un {tipoValor} a un {tipo}", Lexico.log, Lexico.linea, Lexico.columna);
            }

            this.valor = valor;
        }

        public void setValor(float valor, TipoDato maximoTipo)
        {
            if (maximoTipo > tipo)
            {
                throw new Error($"Semántico: No se puede asignar un {maximoTipo} a un {tipo}", Lexico.log, Lexico.linea, Lexico.columna);
            }

            setValor(valor);
        }

        public static TipoDato valorToTipoDato(float valor)
        {
            if (!float.IsInteger(valor))
            {
                return TipoDato.Float;
            }
            if (valor <= 255)
            {
                return TipoDato.Char;
            }
            else if (valor <= 65535)
            {
                return TipoDato.Int;
            }
            else
            {
                return TipoDato.Float;
            }
        }

        public float getValor()
        {
            return valor;
        }
        public string getNombre()
        {
            return nombre;
        }
        public TipoDato GetTipoDato()
        {
            return tipo;
        }
    }
}
