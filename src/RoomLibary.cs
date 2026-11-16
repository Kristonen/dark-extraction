public static class RoomLibary
{
    public static List<RoomTemplate> CreateAll()
    {
        return new()
        {
            RoomTemplateParser.Parse("start", """
                #########
                #.......#
                #.......#
                #E......D
                #.......#
                #.......#
                #########
                """
            ),
            RoomTemplateParser.Parse("test_room", """
                ##D###D##
                #...C...#
                #.......#
                D..###..#
                #..#.#..#
                #...L...#
                #..#.#..#
                #..###..D
                #...C...#
                #.......#
                ##D###D##

                """),
            // RoomTemplateParser.Parse("troll_room", """
            //     #DDDD#
            //     D....D
            //     D....D
            //     D....D
            //     D....D
            //     #DDDD#
            //     """),

            RoomTemplateParser.Parse("corridor_h", """
                #####
                D...D
                #####
                """),

            RoomTemplateParser.Parse("corridor_v", """
                #D#
                #.#
                #.#
                #.#
                #D#
                """),

            RoomTemplateParser.Parse("room_small", """
                ######
                #...C#
                D....#
                #L...#
                ###D##
                """),

            RoomTemplateParser.Parse("hall", """
                #####D#####
                #.........#
                #..#...#..#
                D....C....D
                #..#...#..#
                #.........#
                #####D#####
                """),

            RoomTemplateParser.Parse("dead_end", """
                #####
                #.C.#
                D..L#
                #.C.#
                #####
                """)
        };
    }

    public static RoomTemplate GetCamp()
    {
        return RoomTemplateParser.Parse("camp", """
            ############
            #..........#
            #.........E#
            #..........#
            ############
            """);
    }
}