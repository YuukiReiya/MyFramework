using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class PathExpansion
{
    #region map
    /// <summary>
    /// HACK:微妙.
    /// </summary>
    static Dictionary<string, string> _map = new Dictionary<string, string>()
    {
        {$"$repo", Repository},
        {$"$Repo", Repository},
        {$"$repository", Repository},
        {$"$Repository", Repository},
        {$"$cl", Client},
        {$"$Cl", Client},
        {$"$CL", Client},
        {$"$client", Client},
        {$"$Client", Client},
        {$"$sv", Server},
        {$"$Sv", Server},
        {$"$SV", Server},
        {$"$server", Server},
        {$"$Server", Server},
        {$"$gd", GoogleDrive},
        {$"$GD", GoogleDrive},
    };
    #endregion
    #region property
    public static string Repository { get; private set; } = GetProjectUtil($".git");
    public static string Client { get; private set; } = Path.Combine(GetProjectUtil($"Client"), $"Client");
    public static string Server { get; private set; } = Path.Combine(GetProjectUtil($"Server"), $"Server");

    //public static string GoogleDrive { get; private set; } = Path.Combine(GetProjectUtil($"GoogleDrive"), $"GoogleDrive");
    //HACK:GoogleDriveだけGoogleDrive/GoogleDriveManager ... となっており正常にヒットしない。
    public static string GoogleDrive { get; private set; } = GetProjectUtil($"GoogleDrive");
    public static string ManagePlugin { get; private set; } = Path.Combine(GetProjectUtil($"ManagedPlugins"), $"ManagedPlugins");
    public static string UnmanagePlugin { get; private set; } = Path.Combine(GetProjectUtil($"NativePlugins"), $"NativePlugins");
    #endregion

    /// <summary>
    /// リポジトリをCloneしたディレクトリ.
    /// </summary>
    /// <details>
    /// .gitフォルダが存在するディレクトリを返す。
    /// カレントディレクトリから逆行して探していくので見つからなかったら
    /// 探索元にしたカレントディレクトリを返す.
    /// </details>
    /// <returns></returns>
    [Obsolete("汎用的なGetProjectUtilに統合",false)]
    public static string GetRepository()
    {
        var curdir = Directory.GetCurrentDirectory();
        const string git = ".git";

        int index = -1;
        //1. パスの中に".git"フォルダが含まれているか.
        if (curdir.Contains(git))
        {
            //含まれているならディレクトリを探索せずに終わる.
            //※ディレクトリ探索重いので。
            index = curdir.LastIndexOf(git);
            var result = curdir.Substring(0, index);
            return result;
        }

        //2. 下からディレクトリを順に調べていく.
        var searchDir = curdir;
        while (true)
        {
            // currentdirectoryから.gitフォルダを探す
            var dirs = Directory.GetDirectories(searchDir, "*", SearchOption.TopDirectoryOnly);
            if (dirs.Any(d => d.EndsWith(git)))
            {
                return searchDir;
            }
            // parent
            var info = Directory.GetParent(searchDir);
            if (info != null)
            {
                searchDir = info.FullName;
            }
            else
            {
                Console.WriteLine($"[ERROR]Not found \"{git}\" directory.");
                return curdir;
            }
        }
    }

    static string GetProjectUtil(string searchDirName)
    {
        var curdir = Directory.GetCurrentDirectory();

        int index = -1;
        //1. パスの中に".git"フォルダが含まれているか.
        if (curdir.Contains(searchDirName))
        {
            //含まれているならディレクトリを探索せずに終わる.
            //※ディレクトリ探索重いので。
            index = curdir.LastIndexOf(searchDirName);
            var result = curdir.Substring(0, index);
            return result;
        }

        //2. 下からディレクトリを順に調べていく.
        var searchDir = curdir;
        while (true)
        {
            // currentdirectoryから.gitフォルダを探す
            var dirs = Directory.GetDirectories(searchDir, "*", SearchOption.TopDirectoryOnly);
            //HACK:GoogleDriveだけGoogleDrive/GoogleDriveManager ... となっており正常にヒットしない。
            //他にぶつかるようなら根本的に手を入れる。
            if (dirs.Any(d => d.EndsWith(searchDirName))) 
            {
                return searchDir;
            }
            // parent
            var info = Directory.GetParent(searchDir);
            if (info != null)
            {
                searchDir = info.FullName;
            }
            else
            {
                Console.WriteLine($"[ERROR]Not found \"{searchDir}\" directory. search root dir is {curdir}");
                return curdir;
            }
        }
    }

    /// <summary>
    /// 特定の文字列を変換する.
    /// ※用途的に一度だけでいいので文字列の中に複数回あった場合最初に見つかったもの以外無視.
    /// ($repo/Client/Assets/...) → <リポジトリ>/Client/Assets/...
    /// ($repo/$repo/Assets/...) → <リポジトリ>/$repo/Assets/...
    /// </summary>
    /// <details>
    /// * Repository
    /// * Client
    /// * Server
    /// etc.など各プロジェクトファイルへのパス.
    /// </details>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Convert(string str)
    {
        var conv = str;
        var e = _map.OrderByDescending(x => x.Key.Length).GetEnumerator();
        while (e.MoveNext())
        {
            //var rep = string.Empty;
            var rep = string.Empty;
            switch (e.Current.Key)
            {
                //repository.
                case "$repo":
                case "$Repo":
                case "$repository":
                case "$Repository":
                    rep = Repository;
                    break;
                //client.
                case "$cl":
                case "$Cl":
                case "$CL":
                case "$client":
                case "$Client":
                    rep = Client;
                    break;
                //server.
                case "$sv":
                case "$Sv":
                case "$SV":
                case "$server":
                case "$Server":
                    rep = Server;
                    break;
                //googledrive.
                case "$gd":
                case "$GD":
                    rep = GoogleDrive;
                    break;
            }
            conv = conv.Replace(e.Current.Key, rep);
        }
        return conv;
    }
}
