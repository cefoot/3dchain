using System;
using System.Collections.Generic;
using System.Text;

namespace Chain3D
{
    public class GameOverException : Exception
    {
        public GameOverException(String message)
        {
            Message = message;
        }

        public GameOverException(String message, Location location)
            : this(message)
        {
            OutOfAreaLocation = location;
        }

        public override string ToString()
        {
            return Message;
        }

        public Location OutOfAreaLocation { get; private set; }

        public String Message { get; private set; }
    }
}
