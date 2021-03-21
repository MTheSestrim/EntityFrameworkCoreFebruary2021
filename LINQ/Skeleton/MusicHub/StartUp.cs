namespace MusicHub
{
    using Data;
    using Initializer;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context
                .Producers
                .FirstOrDefault(p => p.Id == producerId)
                .Albums
                .Select(a => new
                {
                    AlbumName = a.Name,
                    a.ReleaseDate,
                    ProducerName = a.Producer.Name,
                    AlbumPrice = a.Price,
                    Songs = a.Songs
                })
                .OrderByDescending(a => a.AlbumPrice)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var album in albums)
            {
                sb.AppendLine($"-AlbumName: {album.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy")}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine($"-Songs:");

                int counter = 1;

                //Extracted songs here because I couldn't get my query to work properly. 
                //I'm pretty sure they could be queried somehow alongside the albums, however.
                var songs = album.Songs.Select(s => new
                {
                    SongName = s.Name,
                    s.Price,
                    Writer = s.Writer.Name
                }).OrderByDescending(s => s.SongName).ThenBy(s => s.Writer).ToList();

                foreach (var song in songs)
                {
                    sb.AppendLine($"---#{counter++}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.Price:F2}");
                    sb.AppendLine($"---Writer: {song.Writer}");
                }
                sb.AppendLine($"-AlbumPrice: {album.AlbumPrice:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            TimeSpan durationComparer = TimeSpan.FromSeconds(duration);

            var songs = context
                .Songs
                .Where(s => s.Duration > durationComparer)
                .Select(s => new
                {
                    SongName = s.Name,
                    Writer = s.Writer.Name,
                    //Unspecified in the problem description as to whether it wants all performers of a song, but it works this way
                    // Checks if performer even exists in the base; if not, it simply places an empty string here. 
                    //Otherwise, the output differs.
                    Performer = s.SongPerformers.FirstOrDefault() != null ? 
                    s.SongPerformers.FirstOrDefault().Performer.FirstName + " " + s.SongPerformers.FirstOrDefault().Performer.LastName : "",
                    AlbumProducer = s.Album.Producer.Name,
                    s.Duration
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.Writer)
                .ThenBy(s => s.Performer)
                .ToList();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < songs.Count; i++)
            {
                sb.AppendLine($"-Song #{i + 1}");
                sb.AppendLine($"---SongName: {songs[i].SongName}");
                sb.AppendLine($"---Writer: {songs[i].Writer}");
                sb.AppendLine($"---Performer: {songs[i].Performer}");
                sb.AppendLine($"---AlbumProducer: {songs[i].AlbumProducer}");
                sb.AppendLine($"---Duration: {songs[i].Duration.ToString("c")}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
