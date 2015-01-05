using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace compiladorUnidad2
{
    public partial class RiegoExpress : Form
    {
        string [] tipoDato = {"int"};
        string [] operador = {"+"};
        string [] palabrasr = {"inicio","fin" };
        string[] expresiones = {"automatico", "encender", "apagar"};
        string[] llaves = {"{","}"};
        string to = "";
        string to2 = "";
        int numApg = 0;
        int nunEnc = 0;

        char[] st = { '\n', '\r', ' ' };
        string[] tabla;
        int nt = 0;
        
       
        public RiegoExpress()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dtgtabla.Rows.Clear();
            txtArduino.Text = " ";
            int lex, sin, sem;
            tabla = txtprograma.Text.Split(st);
            lex = lexico(tabla);
            if (lex == 0) {
                sin = sintactico(tabla);
                if (sin == 0 && lex ==0) {
                    sem = semantico(tabla);
                    if (sem == 0 && sin==0 && lex==0) {
                        traducir(tabla);
                    }
                }
            }
           //traducir(tabla);
            
        }

        //rescata el valor que este entre parentesis en una expresion
        string analizarPar(string parametro) {
            string num = "";
            num = parametro.Substring(1, parametro.Length - 2);
            return num;
        }

       
        
        //traduce el codigo del compilador al de arduino
        void traducir(string[] tabla) {
            txtArduino.Text = null;
            string loop = "", fin = "", variables = "", cuerpo = "", setup = "\r\nvoid setup(){ \r\n";        
               for (int i = 0; i < tabla.Length; i++) {
                if (tabla[i] == "inicio") {
                    loop += "}\r\n void loop(){" + "\r\n";
                }
                if (tabla[i] == "fin") {
                    fin += "}\r\n";
                }
                if (tabla[i] == "encender") {
                    string analizar = analizarPar(tabla[i + 1]);
                    setup += "pinMode(pin1,OUTPUT);\r\n pinMode(pin2,OUTPUT);";
                    variables += "int pin1=9,pin2=10; \r\n";
                    cuerpo += "digitalWrite(pin1,HIGH); \r\n digitalWrite(pin2,LOW); \r\n delay("+analizar+"); ";
                }
                if (tabla[i] == "apagar")
                {
                    string analizar = analizarPar(tabla[i + 1]);
                    cuerpo += "digitalWrite(pin1,LOW); \r\n digitalWrite(pin2,LOW); \r\n delay("+analizar+");";
                }
                   /*
                if (tabla[i] == "humedad")
                {
                    string analizar = analizarPar(tabla[i + 1]);
                    variables+="int Sensor_humedad="+i+";\r\n int lectura=0;\r\n";
                    setup += "pinMode(Sensor_humedad,INPUT);\r\n";
                    cuerpo += "lectura=analogRead(Sensor_humedad,"+analizar+");\r\n";
                }
                    */
                if (tabla[i] == "automatico" && variables==" " && cuerpo==" ") {
                    string analizar = analizarPar(tabla[i + 1]);
                    variables += "float tempC; \r\n int tempPin = 0, pin3=9, pin4=10,led1=7,led2=8; \r\n";
                    setup+= "pinMode(pin3,OUTPUT);\r\n pinMode(pin4,OUTPUT);\r\n pinMode(led1,OUTPUT);\r\npinMode(led2,OUTPUT);";
                    cuerpo += "tempC = analogRead(tempPin); \r\n tempC = (5.0 * tempC * 100.0)/1024.0; \r\n delay("+analizar+");"
                        + "if(tempC>35){digitalWrite(pin3,LOW);\r\r digitalWrite(pin4,HIGH);\r\n digitalWrite(led2,HIGH); digitalWrite(led1,LOW);\r\n delay(" + analizar + ");}"
                        + "else{digitalWrite(pin3,LOW);\r\r digitalWrite(pin4,LOW);\r\n delay(" + analizar + ");}"
                        ;
                    
                }
                if (tabla[i] == "automatico" && variables != " " && cuerpo != " ")
                {
                    string analizar = analizarPar(tabla[i + 1]);
                    variables += "float tempC; \r\n int tempPin = 0, pin3=9, pin4=10,led1=7,led2=8; \r\n";
                    setup += "pinMode(pin3,OUTPUT);\r\n pinMode(pin4,OUTPUT);\r\n pinMode(led1,OUTPUT);\r\npinMode(led2,OUTPUT);";
                    cuerpo += "tempC = analogRead(tempPin); \r\n tempC = (5.0 * tempC * 100.0)/1024.0; \r\n delay(" + analizar + ");"
                        + "if(tempC>35){digitalWrite(pin3,LOW);\r\r digitalWrite(pin4,HIGH);\r\n digitalWrite(led2,HIGH); digitalWrite(led1,LOW);\r\n delay(" + analizar + ");}"
                        + "else{digitalWrite(pin3,LOW);\r\r digitalWrite(pin4,LOW);\r\n delay(" + analizar + ");}"
                        ;
                   
                }

            }
             
            txtArduino.Text=variables+setup+loop+cuerpo+fin;
            //este codigo es para mandar llamar a la funcion que carga al arduino
            string archivo = variables + setup + loop + cuerpo + fin;
               cargar(archivo);
        }
               
        
        //funcion para cargar codigo en el arduino
        void cargar(string archivo) {
            try
            {
                using (StreamWriter writer = new StreamWriter("archivo.ino")) {writer.WriteLine(archivo); }
                System.Diagnostics.Process.Start(@"C:\Users\Chema\Desktop\compiladorUnidad2\compiladorUnidad2\bin\Debug\ejecutar.bat");
            }catch(Exception e){
                MessageBox.Show(e.Message);
            }
        }
        
        //**********************************************************
        int cuentaInicio(string[] tabla){
            //verifica que nomas haya un inicio
            int contador2 = 0;
            for (int i = 0; i < tabla.Length; i++)
            {
                if (tabla[i] == "inicio")
                {
                    contador2++; 
                }
            }
            return contador2;
        }
        //verifica que el parametro de una expresion solo tenga numeros 
        int analizaParametro(string[] tabla) {
            int valor = 0;
            string to = "";
            for (int i = 0; i < tabla.Length; i++) {
                if (tabla[i].CompareTo(" ") == 1)
                {
                to = buscar(tabla[i]);
                    if (to == "parametro")
                    {
                        //dtgtabla.Rows.Add(0,tabla[i]);
                        char[] array = tabla[i].ToCharArray();
                        if (array.Length > 2)//si es dos los parentesis no tienen parametro 
                        {
                            for (int j = 1; j < array.Length - 1; j++)
                            {
                                //dtgtabla.Rows.Add(0,array[j]);//************
                                if (array[j] != '0' && array[j] != '1' && array[j] != '2' && array[j] != '3' && array[j] != '4' && array[j] != '5' && array[j] != '6' && array[j] != '7' && array[j] != '8' && array[j] != '9')
                                {
                                    valor++;
                                }
                            }
                        }
                    }
                }  
            }
            return valor;
        }
        //semantico
        int semantico(string []tabla) {
            int valor = 0;
            
            int flag2 = 0;//cuentan el numero de 'inicio' y 'fin'
            int parametro = 0;
            parametro= analizaParametro(tabla);
            if(parametro>0){
                dtgtabla.Rows.Add(nt + 1, "PARAMETRO INCORRECTO - ERROR SEMANTICO");
                valor++;
            }
            //verifica que la primera palabra sea inicio
            if (tabla[0] != "inicio")
            {
                dtgtabla.Rows.Add(nt+1, "DEBE INICIAR CON 'inicio'- ERROR SEMANTICO");
                valor++;
            }
            int con = 0;
            con = cuentaInicio(tabla);
            if (con != 1)
            {
                dtgtabla.Rows.Add(nt+1, "DEBE TENER SOLO UN 'inicio'- ERROR SEMANTICO");
                valor++;
            }
            if(numApg>0 && nunEnc==0){
                dtgtabla.Rows.Add(nt + 1, "PARA APAGAR DEBE HABER UN ENCENDER- ERROR SEMANTICO");
                numApg = 0;
                nunEnc = 0;
                valor++;
            }
            //verifica que nomas tenga un fin
            for (int i = 0; i < tabla.Length; i++)
            {
                if (tabla[i] == "fin")
                {
                    flag2++;
                    
                }
            }
            if (flag2 > 1)
            {
                dtgtabla.Rows.Add(nt+1, "DEBE TENER SOLO UN 'fin' -ERROR SEMANTICO");
                flag2 = 0;
                valor++;
            }
            //validar que en la ultima palabra sea fin
            if (tabla.Last() != "fin")
            {
                dtgtabla.Rows.Add(nt+1, "DEBE FINALIZAR CON 'fin' -ERROR SEMANTICO");
                valor++;
            }

            return valor;
        }

        

        //verifica que despues de una expresion haya parametro
        int sintactico(string[] tabla) {
            int valor = 0;
            for (int i = 0; i < tabla.Length - 1; i++)
            {
                if (tabla[i].CompareTo(" ") == 1)
                {
                    to = buscar(tabla[i]);
                    if (to.CompareTo("") == 1)
                    {
                        if (to == "expresion")
                        {
                            to2 = buscar(tabla[i + 1]);
                            if (to2 != "parametro")
                            {
                                dtgtabla.Rows.Add(i, "SE ESPERABA PARAMETRO-ERROR SINTACTICO");
                                valor++;
                            }
                        }
                    }
                }
            }
            return valor;
        }

        //identifica los tokens no identificados
        int lexico(string[] tabla)
        {
            int valor = 0;
            for (int i = 0; i < tabla.Length; i++)
            {
                if (tabla[i].CompareTo(" ") == 1)
                {
                    to = buscar(tabla[i]);
                    //cuentan el numero de 'encender' y 'apagar'
                    if (tabla[i] == "encender") { nunEnc++; }
                    if (tabla[i] == "apagar") { numApg++; }
                    if (to.CompareTo("") != 1)
                    {
                       
                            dtgtabla.Rows.Add(tabla[i], "TOKEN NO IDENTIFICADO-ERROR LEXICO");
                            valor++;
                        

                    }
                }
            }
            return valor;
        }
          
        

        string buscar(string palabra)
        {
            string palabra2 = palabra;
            string tipo = "";
            int b = 0;
            //*********
            char[] array = palabra2.ToCharArray();
           
            for (int i = 0; i < expresiones.Length; i++)
            {
                if (expresiones[i].Equals(palabra) && b == 0)
                {
                    tipo = "expresion";
                    b = 1;
                    i = expresiones.Length;

                }


            }
            for (int i = 0; i < palabrasr.Length; i++)
            {
                if (palabrasr[i].Equals(palabra) && b == 0)
                {
                    tipo = "palabraReservada";
                    b = 1;
                    i = palabrasr.Length;
                }

            }
            
           
            for (int i = 0; i < operador.Length; i++)
            {
                if (operador[i].Equals(palabra) && b == 0)
                {
                    tipo = "operador";
                    b = 1;
                    i = operador.Length;
                }

            }
            for (int i = 0; i < tipoDato.Length; i++)
            {
                if (tipoDato[i].Equals(palabra) && b == 0)
                {
                    tipo = "tipoDato";
                    b = 1;
                    i = tipoDato.Length;
                }

            }
            
            for (int i = 0; i < llaves.Length; i++)
            {
                if (llaves[i].Equals(palabra) && b == 0)
                {
                    tipo = "llave";
                    b = 1;
                    i = llaves.Length;
                }

            }
           //*********************
            try
            {
                if (array[0] == '(' && array[array.Length - 1] == ')' && b == 0)
                {
                    tipo = "parametro";
                    b = 1;
                }
            }
            catch {
                MessageBox.Show("la expresion y el parametro deben ir en el mismo renglón");
            }
            
            return tipo;
        }


        //*********************************************

          }
}
