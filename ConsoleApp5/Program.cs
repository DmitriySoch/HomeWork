using System;
using System.Collections.Generic;
using System.Linq;

namespace Task1
{
    class Product //Класс описывающий изделие
    {
        IEnumerable<Node> nodes;
        public Product(IEnumerable<Node> nodes)
            => this.nodes = nodes.ToList();

        public bool IsWorking() //работает когда все узлы работают
            => nodes.All(x => x.IsWorking());

        internal Product Work()
        {   //Заставляет узлы начать работать
            nodes = nodes.Select(x => x.Work());
            return this;
        }
    }

    class Node
    {
        private IEnumerable<Device> devices;
        //Инициализация узла, создаются устройства в узле
        public Node(IEnumerable<double> devicesProbability) =>
            this.devices = devicesProbability.Select(x => new Device(x));
        //Работает когда хотя-бы одно из устройств работает
        public bool IsWorking()
            => this.devices.Any(x => x.IsWorking());

        internal Node Work()
        {   //Заставляет устройства начать работать
            devices = devices.Select(x => x.Work());
            return this;
        }
    }

    class Device
    {
        public readonly double workProbability;
        public bool isWorking = true;
        //Инициализация устройства
        public Device(double workProbability)
            => this.workProbability = workProbability;

        public void Stop() => this.isWorking = false;

        public bool IsWorking() => this.isWorking;

        internal Device Work()
        {
            if (Program.random.NextDouble() > workProbability)
            {   //Устройство ломается
                this.Stop();
            }
            return this;
        }
    }

    class Program
    {
        public static Random random = new Random();
        public static double Task1()
        {
            var result = 0;
            for (var i = 0; i < 100; i++)
            {   //Создаем узлы и помещаем их в изделие
                var firstNode = new Node(new[] { 0.8, 0, 0.95 });
                var secondNode = new Node(new[] { 0.85 });
                var thirdNode = new Node(new[] { 0.9, 0.7 });
                var product = new Product(new[] { firstNode, secondNode, thirdNode });
                //Запускаем изделие и проверяем что оно не сломалось,
                //если сломалось увеличиваем счетчик на 1
                result += product.Work().IsWorking() ? 0 : 1;
            }
            //Возвращаем вероятность безотказной работы
            return 1 - (result / 100.0);
        }
    }
}

namespace Task2
{
    class Device
    {
        private double detectionСhance;
        private (double, double) errorChance;
        private bool isWorking = true;

        public Device(double detectionСhance, (double, double) errorChance)
        {
            this.detectionСhance = detectionСhance;
            this.errorChance = errorChance;
        }

        public void Work(double rnd)
        {
            if (rnd >= errorChance.Item1 && rnd < errorChance.Item2)
            {   //На устройстве произошел сбой
                isWorking = false;
            }
            if (Program.random.NextDouble() < detectionСhance)
            {
                if (isWorking == false)//Произощел сбой и его обнаружили
                    throw new Exception("Ошибка была обнаружена");
            }
        }
    }

    class Program
    {
        public static Random random = new Random();
        public static double Task2()
        {
            var result = 0;
            //Определяем вероятности сбоя устройств
            var probability = CountProbability(2, 3, 5);
            for (int i = 0; i < 100; i++)
            {
                //Инициализируем устройства
                var devices = new List<Device>()
                {
                    new Device(0.8,(0, probability[0])),
                    new Device(0.9, (probability[0], probability[0]+ probability[1])),
                    new Device(0.95, (probability[2], 1))
                };
                try
                {
                    var rnd = Program.random.NextDouble();
                    //Начинает работать
                    devices.ForEach(x => x.Work(rnd));
                }
                catch (Exception)
                {   //Был сбой который обнаружили
                    result++;
                }
            }
            //Возвращаем обнаружения сбоя
            return result / 100.0;
        }

        public static double[] CountProbability(params int[] args)
            => args.Select(x => x / (double)args.Sum()).ToArray();
    }
}

namespace HomeWork
{
    class Program
    {
        static void Main(string[] args)
        {
            //Выполняем Задание 1
            Exec(Task1.Program.Task1, "Задание 1");
            //Выполняем Задание 2
            Exec(Task2.Program.Task2, "Задание 2");
            Console.ReadKey();
        }

        public static void Exec(Func<double> action, string task)
        {   //Проводим 100 моделяций заданного эксперимента
            Console.WriteLine(task);
            Console.Write("Искомая вероятность = ");
            var middle = 0.0;
            for (int i = 0; i < 100; i++)
            {
                middle += action();
            }
            Console.WriteLine(middle / 100);
        }
    }
}