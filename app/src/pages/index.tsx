import { GetServerSidePropsContext } from "next";
import Head from "next/head";

const Index = ({

} : {

}) => {
    return (
        <>
            <Head>
                <title>Ntickets</title>
            </Head>
            <main>
                <h1>Ntickets</h1>
            </main>
        </>
    );
}

export async function getServerSideProps(context: GetServerSidePropsContext) {
    return {
        props : {

        }
    }
}

export default Index;