using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fin.promise {
  interface IPromise<PROMISED_T> {
    IPromise<CONVERTED_T> Then<CONVERTED_T>(Func<PROMISED_T, IPromise<CONVERTED_T>> onResolved);

    IPromise<CONVERTED_T> Then<CONVERTED_T>(
      Func<PROMISED_T, IPromise<CONVERTED_T>> onResolved,
      Func<PROMISED_T, IPromise<CONVERTED_T>> onRejected
    );
  }
}
