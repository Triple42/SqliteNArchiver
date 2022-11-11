using SqliteNArchiver;



if (Directory.Exists("data2"))
    Directory.Delete("data2", true);

using var nArchive = new NArchive("archive");

if (System.IO.File.Exists(nArchive.DbName))
    System.IO.File.Delete(nArchive.DbName);


nArchive.CreateArchive();
nArchive.AddFile(new FileInfo("data/photo_identite.jpg"));
nArchive.AddFile(new FileInfo("data/carte_identite_recto.jpg"));