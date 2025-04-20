import { ReactNode } from "react";
import { createSignalRContext } from "react-signalr/signalr";
import { store } from "@shared/store/store";
import { observer } from "mobx-react-lite";

const SignalRContext = createSignalRContext();

export const SignalrProvider = observer(({children}: {children: ReactNode}) => {
    return (
      <SignalRContext.Provider
        connectEnabled={!!store.test_guid}
        url={window.location.origin + "/api/realtime"}
      >
        {children}
      </SignalRContext.Provider>
    );
})

