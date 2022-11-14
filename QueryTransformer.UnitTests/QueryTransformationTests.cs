namespace QueryTransformer.UnitTests
{
    public class QueryTransformationTests
    {
        private Pipeline<string, string> _sut;

        [OneTimeSetUp]
        public void SetupOnce()
        {
            _sut = new Pipeline<string, string>(
                input =>
                    input
                        .Step(new PipelineStep<string, char[]>(str => str.ToCharArray()))
                        .Step(new RegexReplaceStep(@"[A-Z]+", (match) => match.ToLower()))
                        .Step(new RegexReplaceStep(@"^\s+", string.Empty))
                        .Step(new RegexReplaceStep(@"\s+$", string.Empty))
                        .Step(new RegexReplaceStep(@"\r\n", " "))
                        .Step(new RegexReplaceStep(@"\s{2,}", " "))
                        .Step(new RegexReplaceStep(@"\(\s+", "("))
                        .Step(new RegexReplaceStep(@"\s+\)", ")"))
                        .Step(new RegexReplaceStep(@"\((\s*"".+""\s*\,*\s*)+\)", "(...)"))
                        .Step(new RegexReplaceStep(@""".+""", "?"))
                        .Step(new PipelineStep<char[], string>(chars => new string(chars)))
            ); 
        }

        [Test]
        public void UppercaseLetters_ShouldBeTransformedToLowercase()
        {
            var input = "SELECT * FROM TABLE";
            var result = _sut.Execute(input);

            Assert.That(result, Is.EqualTo("select * from table"));
        }

        [Test]
        public void ExcessSpaces_ShouldBeRemoved()
        {            
            var input = "  SELECT *     FROM TABLE ";
            var result = _sut.Execute(input);

            Assert.That(result, Is.EqualTo("select * from table"));
        }

        [Test]
        public void Parameters_ShouldBeReplacedWithQuestionMarks()
        {
            var input = "  SELECT *     FROM TABLE WHERE Id = \"123\"   ";
            var result = _sut.Execute(input);

            Assert.That(result, Is.EqualTo("select * from table where id = ?"));
        }

        [Test]
        public void ParametersInBraces_ShouldBeReplacedWithDots()
        {
            var input = "  SELECT *   FROM TABLE WHERE Id IN ( \"1\", \"222\", \"1015\" )  ";
            var result = _sut.Execute(input);

            Assert.That(result, Is.EqualTo("select * from table where id in (...)"));
        }        

        [Test]
        public void Query_ShouldBeTransformedToCanonicalForm_1()
        {
            var input = @"SELECT
              id, name
            FROM users
            WHERE
              family = ""Petrov"" and
              (
                group_id = ""1"" or manager_id in (""1"", ""3"", ""8"", ""92"")
              ); ";

            var result = _sut.Execute(input);
            Assert.That(result, Is.EqualTo("select id, name from users where family = ? and (group_id = ? or manager_id in (...));"));
        }

        [Test]
        public void Query_ShouldBeTransformedToCanonicalForm_2()
        {
            var input = @"select id from tasks
            where query_part
            in ( ""; "" , "" in ( ?, ? )"",
            ""in"" ) 
            and task_owner = ""Vasily""; ";

            var result = _sut.Execute(input);
            Assert.That(result, Is.EqualTo("select id from tasks where query_part in (...) and task_owner = ?;"));
        }

        [Test]
        public void Query_ShouldBeTransformedToCanonicalForm_3()
        {
            var input = @"select t.id
            from topics t
            inner join readers r
            on r.t_id = t.id
            where (
            ifnull( t.ref, ""n"" ) = ""a""
            or t.name = ""messages""
            or t.name like ""%'""
            ); ";

            var result = _sut.Execute(input);
            Assert.That(result, Is.EqualTo("select t.id from topics t inner join readers r on r.t_id = t.id where (ifnull(t.ref, ?) = ? or t.name = ? or t.name like ?);"));
        }
    }
}