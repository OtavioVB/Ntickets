import { Fragment } from "react";

export default function MyApp({ Component, pageProps }) {

    return (
        <Fragment>
            <Component {...pageProps} />
        </Fragment>
    )
}