import { Fragment } from "react";
import '@/styles/global.css';

export default function MyApp({ Component, pageProps }) {

    return (
        <Fragment>
            <Component {...pageProps} />
        </Fragment>
    )
}