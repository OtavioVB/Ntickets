import { GetServerSidePropsContext } from "next";

const Index = ({

} : {

}) => {
    return (
        <h1>Ntickets</h1>
    );
}

export async function getServerSideProps(context: GetServerSidePropsContext) {
    return {
        props : {

        }
    }
}

export default Index;