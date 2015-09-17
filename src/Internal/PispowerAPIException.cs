using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pispower.Video.RestfulApi.Internal
{
    class PispowerAPIException : ApplicationException
    {
        private int code;

        private String message;

        public PispowerAPIException(int code, String message)
	    {
            this.message = message;
		    this.code = code;
	    }

	    public PispowerAPIException(String message)
	    {
            this.message = message;
	    }

	    /**
	     * @return the code
	     */
        public int getCode()
	    {
		    return code;
	    }
    }
}
