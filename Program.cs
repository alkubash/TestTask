using System.Diagnostics;
using System.Text;

Console.WriteLine("Введите путь к текстовому файлу:");
string path = Console.ReadLine();

Stopwatch stopWatch = new Stopwatch();
stopWatch.Start();
try
{
    using (FileStream fstream = System.IO.File.OpenRead(path))
    {
        byte[] buffer = new byte[fstream.Length];
        await fstream.ReadAsync(buffer, 0, buffer.Length);
        string textFromFile = Encoding.Default.GetString(buffer);
        Console.WriteLine($"Текст из файла: {textFromFile}");

        var paragraphMarker = Environment.NewLine + Environment.NewLine;
        var paragraphs = textFromFile.Split(new[] { paragraphMarker },
                                        StringSplitOptions.RemoveEmptyEntries);
        var countParagraphs = 10/paragraphs.Length;
        await Task.Run(() =>
        {
            paragraphs.AsParallel()
                .WithDegreeOfParallelism(10)
                .ForAll(paragraph =>
                {
                    var words = paragraph.Split(new[] { ' ' },
                                  StringSplitOptions.RemoveEmptyEntries)
                                 .Select(w => w.Trim());
                    var groups = SwimParts(paragraph, 3)
                        .Where(str => str.All(ch => char.IsLetter(ch)))
                        .GroupBy(str => str);
                    Console.WriteLine(string.Join
            (
                Environment.NewLine,
                groups.OrderByDescending(gr => gr.Count()).Take(countParagraphs).Select(gr => $"\"{gr.Key}\" встретилось {gr.Count()} раз")
            ));
                });
            
        });


    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    TimeSpan ts = stopWatch.Elapsed;
    string elapsedTime = String.Format("{0:00} часов, {1:00} минут, {2:00} секунд, {3:000} миллисекунд",
                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
    Console.WriteLine("Время выполнения программы: " + elapsedTime);
}

static IEnumerable<string> SwimParts(string source, int length)
{
    for (int i = length; i <= source.Length; i++)
        yield return source.Substring(i - length, length);
}


